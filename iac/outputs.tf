output "ecr_repository_url" {
  value = aws_ecr_repository.llama.repository_url
}

output "alb_dns_name" {
  value = aws_lb.llama.dns_name
}

output "cluster_name" {
  value = aws_ecs_cluster.llama.name
}

output "model_name" {
  value = var.model_name
}
