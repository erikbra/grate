locals {
  ORACLE_ROOT_PASSWORD = random_password.oracle_admin.result
}

resource "azurerm_container_app" "oracle" {

  count = length(local.oracle_images)

  name                         = "${replace(replace(replace(replace(local.oracle_images[count.index], "gvenzl/", ""), "-faststart", ""), ":", "-"), ".", "-")}"
  container_app_environment_id = azurerm_container_app_environment.grate-tests.id
  resource_group_name          = data.azurerm_resource_group.grate-integration-tests.name
  revision_mode                = "Single"

  template {
    min_replicas  = 1
    max_replicas  = 1

    # tcp_scale_rule {
    #     name = "scale-on-one-request"
    #     concurrent_requests = 1 
    # }

    container {
      name          = "oracle"
      image         = local.oracle_images[count.index]
      cpu           = 2.00
      memory        = "4Gi"

      # ref the Docker image documentation: https://github.com/gvenzl/oci-oracle-xe
      env {
        name = "ORACLE_PASSWORD" 
        value = local.ORACLE_ROOT_PASSWORD
      }

      env {
        name = "ORACLE_PWD" 
        value = local.ORACLE_ROOT_PASSWORD
      }

      env {
        name = "ORACLE_PDB" 
        value = "FREEPDB1"
      }

      liveness_probe {
        failure_count_threshold = 10
        initial_delay = 30
        port = 1521
        timeout = 5
        transport = "TCP"
      }

      readiness_probe {
        failure_count_threshold = 10
        port = 1521
        timeout = 5
        transport = "TCP"
      }

      startup_probe {
        failure_count_threshold = 10
        port = 1521
        timeout = 5
        transport = "TCP"
      }

    }
  }
  ingress {
    external_enabled = true
    exposed_port = (1521 + count.index)
    target_port = 1521 
    transport = "tcp"
    traffic_weight {
      percentage = 100
      latest_revision = true
    }
  }
}
