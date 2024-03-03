resource "azurerm_storage_container" "grate-tests-setup" {
  name                 = "grate-tests-setup"
  storage_account_name = "grateconfig"
}

data "azurerm_storage_account" "grateconfig" {
  name                 = "grateconfig"
  resource_group_name  = "grate-tests-infra"
}
