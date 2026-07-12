terraform {
    backend "azurerm" {
        resource_group_name  = "serverless-rg"
        storage_account_name = "tfstateserverless0046"
        container_name       = "tfstate"
        key                  = "terraform.tfstate"
    }
}