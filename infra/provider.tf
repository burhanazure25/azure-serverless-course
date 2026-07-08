terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.46.0"
    }
  }
}

provider "azurerm" {
  subscription_id = "60896ef9-21a0-40f0-9176-963a8881a093"

  features {}
}