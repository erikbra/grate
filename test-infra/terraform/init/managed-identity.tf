resource "azurerm_user_assigned_identity" "integration-tests-mi" {
  location            = azurerm_resource_group.grate-integration-tests.location
  name                = "integration-tests-mi"
  resource_group_name = azurerm_resource_group.grate-integration-tests.name
}

resource "azurerm_role_assignment" "integration-tests-rg-contributor" {
  scope                = azurerm_resource_group.grate-integration-tests.id
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.integration-tests-mi.principal_id
}

resource "azurerm_role_assignment" "terraform-state-contributor" {
  scope                = azurerm_storage_container.grate-tests-setup.resource_manager_id
  role_definition_name = "Storage Blob Data Owner"
  principal_id         = azurerm_user_assigned_identity.integration-tests-mi.principal_id
}

resource "azurerm_federated_identity_credential" "integration-tests" {
  name                = "${var.github_organisation_target}-${var.github_repository_name}-integration-tests"
  resource_group_name = azurerm_resource_group.grate-integration-tests.name
  audience            = [local.default_audience_name]
  issuer              = local.github_issuer_url
  parent_id           = azurerm_user_assigned_identity.integration-tests-mi.id
  subject             = "repo:${var.github_organisation_target}/${var.github_repository_name}:environment:integration-tests"
}
