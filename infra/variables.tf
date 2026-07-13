variable "location" {
  description = "The Azure region to deploy resources in."
  type        = string
  default     = "southeastasia"
}

variable "project_name" {
  description = "The name of the project."
  type        = string
  default     = "order-system"
}

variable "environment" {
  description = "The environment for the deployment (e.g., dev, staging, prod)."
  type        = string
  default     = "dev"
}

variable "subscription_id" {
  description = "The Azure subscription ID where resources will be deployed."
  type        = string
  default     = ""
}