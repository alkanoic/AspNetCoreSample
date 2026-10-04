terraform {
  required_version = ">= 1.6.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }
}

provider "aws" {
  region = var.region
}

locals {
  name          = "${var.name_prefix}-llama-cpp"
  model_s3_path = replace(var.model_s3_uri, "s3://", "")
}

data "aws_ssm_parameter" "gpu_ami" {
  name = "/aws/service/ecs/optimized-ami/amazon-linux-2023/gpu/recommended"
}

resource "aws_ecr_repository" "llama" {
  name                 = local.name
  image_tag_mutability = "IMMUTABLE"
  force_delete         = false

  image_scanning_configuration {
    scan_on_push = true
  }

  encryption_configuration {
    encryption_type = "AES256"
  }
}

resource "aws_ecr_lifecycle_policy" "llama" {
  repository = aws_ecr_repository.llama.name
  policy = jsonencode({
    rules = [{
      rulePriority = 1
      description  = "Retain the 5 most recent images"
      selection = {
        tagStatus   = "any"
        countType   = "imageCountMoreThan"
        countNumber = 5
      }
      action = { type = "expire" }
    }]
  })
}

resource "aws_cloudwatch_log_group" "llama" {
  name              = "/ecs/${local.name}"
  retention_in_days = 30
}

resource "aws_security_group" "alb" {
  name_prefix = "${local.name}-alb-"
  description = "HTTP ingress for llama.cpp from on-premises"
  vpc_id      = var.vpc_id

  ingress {
    description = "On-premises clients via Direct Connect"
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = var.allowed_cidrs
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "alb_to_task" {
  type                     = "egress"
  description              = "ECS tasks"
  from_port                = 8080
  to_port                  = 8080
  protocol                 = "tcp"
  security_group_id        = aws_security_group.alb.id
  source_security_group_id = aws_security_group.task.id
}

resource "aws_security_group" "task" {
  name_prefix = "${local.name}-task-"
  description = "llama.cpp tasks"
  vpc_id      = var.vpc_id

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "task_from_alb" {
  type                     = "ingress"
  description              = "ALB health checks and inference"
  from_port                = 8080
  to_port                  = 8080
  protocol                 = "tcp"
  security_group_id        = aws_security_group.task.id
  source_security_group_id = aws_security_group.alb.id
}

resource "aws_security_group" "instance" {
  name_prefix = "${local.name}-ec2-"
  description = "ECS GPU container instances"
  vpc_id      = var.vpc_id

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "instance_egress" {
  type              = "egress"
  description       = "ECS, ECR, SSM and CloudWatch through VPC egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  security_group_id = aws_security_group.instance.id
  cidr_blocks       = ["0.0.0.0/0"]
}

resource "aws_security_group_rule" "task_egress" {
  type              = "egress"
  description       = "ECR, CloudWatch and Secrets Manager through VPC egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  security_group_id = aws_security_group.task.id
  cidr_blocks       = ["0.0.0.0/0"]
}

resource "aws_iam_role" "instance" {
  name = "${local.name}-instance"
  assume_role_policy = jsonencode({
    Version   = "2012-10-17"
    Statement = [{ Effect = "Allow", Principal = { Service = "ec2.amazonaws.com" }, Action = "sts:AssumeRole" }]
  })
}

resource "aws_iam_role_policy_attachment" "instance_ecs" {
  role       = aws_iam_role.instance.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

resource "aws_iam_role_policy_attachment" "instance_ssm" {
  role       = aws_iam_role.instance.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_instance_profile" "instance" {
  name = "${local.name}-instance"
  role = aws_iam_role.instance.name
}

resource "aws_iam_role" "execution" {
  name = "${local.name}-execution"
  assume_role_policy = jsonencode({
    Version   = "2012-10-17"
    Statement = [{ Effect = "Allow", Principal = { Service = "ecs-tasks.amazonaws.com" }, Action = "sts:AssumeRole" }]
  })
}

resource "aws_iam_role_policy_attachment" "execution" {
  role       = aws_iam_role.execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy" "secret" {
  count = var.api_key_secret_arn == null ? 0 : 1
  name  = "llama-api-key"
  role  = aws_iam_role.execution.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = ["secretsmanager:GetSecretValue"]
      Resource = var.api_key_secret_arn
    }]
  })
}

resource "aws_iam_role" "task" {
  name = "${local.name}-task"
  assume_role_policy = jsonencode({
    Version   = "2012-10-17"
    Statement = [{ Effect = "Allow", Principal = { Service = "ecs-tasks.amazonaws.com" }, Action = "sts:AssumeRole" }]
  })
}

resource "aws_iam_role_policy" "model_s3" {
  name = "llama-model-s3"
  role = aws_iam_role.task.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = ["s3:GetObject", "s3:GetObjectVersion"]
      Resource = "arn:aws:s3:::${local.model_s3_path}"
    }]
  })
}

resource "aws_ecs_cluster" "llama" {
  name = local.name
}

resource "aws_launch_template" "gpu" {
  name_prefix            = "${local.name}-"
  image_id               = jsondecode(data.aws_ssm_parameter.gpu_ami.value).image_id
  instance_type          = "g6.xlarge"
  update_default_version = true

  iam_instance_profile {
    name = aws_iam_instance_profile.instance.name
  }

  network_interfaces {
    associate_public_ip_address = false
    security_groups             = [aws_security_group.instance.id]
  }

  metadata_options {
    http_endpoint               = "enabled"
    http_tokens                 = "required"
    http_put_response_hop_limit = 2
  }

  block_device_mappings {
    device_name = "/dev/xvda"
    ebs {
      volume_size           = 100
      volume_type           = "gp3"
      encrypted             = true
      delete_on_termination = true
    }
  }

  user_data = base64encode(<<-EOF
    #!/bin/bash
    printf '%s\n' 'ECS_CLUSTER=${aws_ecs_cluster.llama.name}' 'ECS_ENABLE_GPU_SUPPORT=true' >> /etc/ecs/ecs.config
  EOF
  )

  tag_specifications {
    resource_type = "instance"
    tags          = { Name = local.name }
  }
}

resource "aws_autoscaling_group" "gpu" {
  name                      = local.name
  min_size                  = 1
  max_size                  = var.max_instances
  desired_capacity          = 1
  vpc_zone_identifier       = var.private_subnet_ids
  health_check_type         = "EC2"
  health_check_grace_period = 300
  protect_from_scale_in     = true

  launch_template {
    id      = aws_launch_template.gpu.id
    version = "$Latest"
  }

  tag {
    key                 = "AmazonECSManaged"
    value               = "true"
    propagate_at_launch = true
  }

  lifecycle {
    ignore_changes = [desired_capacity]
  }
}

resource "aws_ecs_capacity_provider" "gpu" {
  name = local.name

  auto_scaling_group_provider {
    auto_scaling_group_arn         = aws_autoscaling_group.gpu.arn
    managed_termination_protection = "ENABLED"

    managed_scaling {
      status                    = "ENABLED"
      target_capacity           = 100
      minimum_scaling_step_size = 1
      maximum_scaling_step_size = 1
    }
  }
}

resource "aws_ecs_cluster_capacity_providers" "gpu" {
  cluster_name       = aws_ecs_cluster.llama.name
  capacity_providers = [aws_ecs_capacity_provider.gpu.name]

  default_capacity_provider_strategy {
    capacity_provider = aws_ecs_capacity_provider.gpu.name
    weight            = 1
  }
}

resource "aws_ecs_task_definition" "llama" {
  family                   = local.name
  requires_compatibilities = ["EC2"]
  network_mode             = "awsvpc"
  cpu                      = "3072"
  memory                   = "12288"
  execution_role_arn       = aws_iam_role.execution.arn
  task_role_arn            = aws_iam_role.task.arn

  container_definitions = jsonencode([{
    name                 = "llama-server"
    image                = "${aws_ecr_repository.llama.repository_url}:${var.image_tag}"
    essential            = true
    cpu                  = 3072
    memory               = 12288
    portMappings         = [{ containerPort = 8080, hostPort = 8080, protocol = "tcp" }]
    resourceRequirements = [{ type = "GPU", value = "1" }]
    environment = [
      { name = "MODEL_S3_URI", value = var.model_s3_uri },
      { name = "MODEL_SHA256", value = var.model_sha256 },
      { name = "LLAMA_ARG_MODEL", value = "/models/Ternary-Bonsai-2-27B-PQ2_0.gguf" },
      { name = "LLAMA_ARG_ALIAS", value = var.model_name },
      { name = "LLAMA_ARG_CTX_SIZE", value = tostring(var.context_size) },
      { name = "LLAMA_ARG_N_PARALLEL", value = "1" },
      { name = "LLAMA_ARG_N_GPU_LAYERS", value = "99" },
      { name = "LLAMA_ARG_JINJA", value = "true" },
      { name = "LLAMA_ARG_OFFLINE", value = "true" },
      { name = "NVIDIA_DRIVER_CAPABILITIES", value = "compute,utility" }
    ]
    secrets = var.api_key_secret_arn == null ? [] : [{ name = "LLAMA_API_KEY", valueFrom = var.api_key_secret_arn }]
    logConfiguration = {
      logDriver = "awslogs"
      options = {
        awslogs-group         = aws_cloudwatch_log_group.llama.name
        awslogs-region        = var.region
        awslogs-stream-prefix = "llama"
      }
    }
  }])
}

resource "aws_lb" "llama" {
  name                       = substr(local.name, 0, 32)
  load_balancer_type         = "application"
  internal                   = true
  subnets                    = var.private_subnet_ids
  security_groups            = [aws_security_group.alb.id]
  idle_timeout               = 600
  drop_invalid_header_fields = true
}

resource "aws_lb_target_group" "llama" {
  name                 = substr(local.name, 0, 32)
  port                 = 8080
  protocol             = "HTTP"
  target_type          = "ip"
  vpc_id               = var.vpc_id
  deregistration_delay = 60

  health_check {
    path                = "/health"
    matcher             = "200"
    interval            = 30
    timeout             = 10
    healthy_threshold   = 2
    unhealthy_threshold = 10
  }
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.llama.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.llama.arn
  }
}

resource "aws_ecs_service" "llama" {
  name                               = local.name
  cluster                            = aws_ecs_cluster.llama.id
  task_definition                    = aws_ecs_task_definition.llama.arn
  desired_count                      = 1
  health_check_grace_period_seconds  = 600
  deployment_minimum_healthy_percent = 0
  deployment_maximum_percent         = 100
  enable_execute_command             = false

  capacity_provider_strategy {
    capacity_provider = aws_ecs_capacity_provider.gpu.name
    weight            = 1
  }

  network_configuration {
    subnets          = var.private_subnet_ids
    security_groups  = [aws_security_group.task.id]
    assign_public_ip = false
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.llama.arn
    container_name   = "llama-server"
    container_port   = 8080
  }

  depends_on = [aws_ecs_cluster_capacity_providers.gpu, aws_lb_listener.http, aws_iam_role_policy.secret, aws_security_group_rule.task_from_alb, aws_security_group_rule.alb_to_task, aws_security_group_rule.instance_egress, aws_security_group_rule.task_egress]
}
