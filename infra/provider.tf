terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.46.0"
    }
  }
}

provider "azurerm" {
  subscription_id = "63419108-a118-4654-92f9-17b18d5968bb"

  features {}
}