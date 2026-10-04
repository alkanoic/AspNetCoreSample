variable "region" {
  type    = string
  default = "ap-northeast-1"
}

variable "name_prefix" {
  type    = string
  default = "bonsai"

  validation {
    condition     = can(regex("^[a-z][a-z0-9-]{0,16}$", var.name_prefix))
    error_message = "Use 1-17 lowercase alphanumeric or hyphen characters, starting with a letter."
  }
}

variable "vpc_id" {
  type = string
}

variable "private_subnet_ids" {
  type = list(string)

  validation {
    condition     = length(distinct(var.private_subnet_ids)) >= 2
    error_message = "Provide private subnets in at least two Availability Zones."
  }
}

variable "allowed_cidrs" {
  type = list(string)

  validation {
    condition     = length(var.allowed_cidrs) > 0 && alltrue([for cidr in var.allowed_cidrs : can(cidrhost(cidr, 0))])
    error_message = "Provide at least one valid on-premises CIDR reachable through Direct Connect."
  }
}

variable "api_key_secret_arn" {
  type    = string
  default = null
}

variable "model_s3_uri" {
  type = string

  validation {
    condition     = can(regex("^s3://[^/]+/.+\\.gguf$", var.model_s3_uri))
    error_message = "Set model_s3_uri to the S3 URI of the GGUF file (s3://bucket/key.gguf)."
  }
}

variable "model_sha256" {
  type    = string
  default = "3907dc1658db1f78a9826bf8d5bcb8dc65db0d466388937af57f2294fae62ec1"
}

variable "image_tag" {
  type = string

  validation {
    condition     = can(regex("^[a-zA-Z0-9_][a-zA-Z0-9_.-]{0,127}$", var.image_tag)) && var.image_tag != "latest"
    error_message = "Use a versioned, immutable ECR image tag (not latest)."
  }
}

variable "model_name" {
  type    = string
  default = "bonsai-2"
}

variable "context_size" {
  type    = number
  default = 8192

  validation {
    condition     = var.context_size >= 512 && var.context_size <= 32768
    error_message = "Set context_size between 512 and 32768 after validating GPU memory usage."
  }
}

variable "max_instances" {
  type    = number
  default = 1

  validation {
    condition     = var.max_instances >= 1 && var.max_instances <= 10 && floor(var.max_instances) == var.max_instances
    error_message = "Set max_instances to a whole number between 1 and 10."
  }
}
