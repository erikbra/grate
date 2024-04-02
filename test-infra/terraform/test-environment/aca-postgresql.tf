locals {
  POSTGRES_PASSWORD = random_password.postgresql_admin.result
}

resource "azurerm_container_app" "posgresql" {

  count = length(local.postgresql_images)

  name                         = "${replace(replace(local.postgresql_images[count.index], ":", "-"), ".", "-")}"
  container_app_environment_id = azurerm_container_app_environment.grate-tests.id
  resource_group_name          = data.azurerm_resource_group.grate-integration-tests.name
  revision_mode                = "Single"

  template {
    min_replicas  = 0
    max_replicas  = 1

    tcp_scale_rule {
        name = "scale-on-one-request"
        concurrent_requests = 1 
    }

    container {
      name          = "postgresql"
      image         = local.postgresql_images[count.index]
      cpu           = 0.25
      memory        = "0.5Gi"

      env {
        name = "POSTGRES_PASSWORD" 
        value = local.POSTGRES_PASSWORD
      }

    }
  }
  ingress {
    external_enabled = true
    exposed_port = (5432 + count.index)
    target_port = 5432 
    transport = "tcp"
    traffic_weight {
      percentage = 100
      latest_revision = true
    }
  }
}