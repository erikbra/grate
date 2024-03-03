resource "azurerm_network_security_group" "aca-env" {
  name                = "aca-env-nsg"
  location            = data.azurerm_resource_group.grate-integration-tests.location
  resource_group_name = data.azurerm_resource_group.grate-integration-tests.name
}

resource "azurerm_virtual_network" "aca-env" {
  name                = "aca-env-vnet"
  location            = data.azurerm_resource_group.grate-integration-tests.location
  resource_group_name = data.azurerm_resource_group.grate-integration-tests.name
  address_space       = ["10.1.0.0/16"]
#   dns_servers         = ["10.0.0.4", "10.0.0.5"]
}

resource "azurerm_subnet" "aca-env" {
    name           = "aca-env-infra-subnet"
    virtual_network_name = azurerm_virtual_network.aca-env.name
    resource_group_name = azurerm_virtual_network.aca-env.resource_group_name
    address_prefixes = [ "10.1.0.0/21" ]
    #security_group = azurerm_network_security_group.aca-env.id
}