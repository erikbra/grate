terraform {
  backend "azurerm" {
    resource_group_name  = "grate-tests-infra"
    storage_account_name = "grateconfig"
    container_name       = "grate-tests-infra-setup"
    key                  = "init.tfstate"
  }

  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
    }
    azuread = {}
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {
}
