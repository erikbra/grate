resource "azurerm_resource_group" "grate-integration-tests" {
  name     = "grate-integration-tests"
  location = "eastus"
}

# data "azurerm_resource_group" "grate-tests-infra" {
#   name     = "grate-tests-infra"
# }