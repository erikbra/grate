resource "azurerm_container_app_environment" "grate-tests" {
  name                       = "grate-integration-tests"
  location                   = data.azurerm_resource_group.grate-integration-tests.location
  resource_group_name        = data.azurerm_resource_group.grate-integration-tests.name

  infrastructure_subnet_id = azurerm_subnet.aca-env.id
}
