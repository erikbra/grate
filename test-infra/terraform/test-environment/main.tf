terraform {
  backend "azurerm" {
    resource_group_name  = "grate-tests-infra"
    storage_account_name = "grateconfig"
    container_name       = "grate-tests-setup"
    key                  = "test-environment.tfstate"

    use_oidc             = true
    use_azuread_auth     = true
  }

  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
    }
    azuread = {}
  }
}

provider "azurerm" {
  #use_oidc = true
  skip_provider_registration = true
  features {}
}

provider "azuread" {
}

data "azurerm_client_config" "current" {}
