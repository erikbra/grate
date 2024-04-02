locals {
  SQLSERVER_SA_PASSWORD = random_password.mssql_admin.result
}

resource "azurerm_container_app" "sqlserver" {

  count = length(local.sqlserver_images)

  
  name                         = "${replace(replace(replace(replace(local.sqlserver_images[count.index], "mcr.microsoft.com/", ""), "/", "-"), ":", "-"), ".", "-")}"
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
      name          = "sqlserver"
      image         = local.sqlserver_images[count.index]
      cpu           = 2.0
      memory        = "4Gi"

      env {
        name = "MSSQL_SA_PASSWORD" 
        value = local.SQLSERVER_SA_PASSWORD
      }

      env {
        name = "SQLCMDPASSWORD" 
        value = local.SQLSERVER_SA_PASSWORD
      }

      env {
        name = "ACCEPT_EULA" 
        value = "Y"
      }

      env {
        name = "MSSQL_COLLATION" 
        value = "Danish_Norwegian_CI_AS"
      }

      liveness_probe {
        failure_count_threshold = 10
        initial_delay = 30
        port = 1433
        timeout = 5
        transport = "TCP"
      }

      readiness_probe {
        failure_count_threshold = 10
        port = 1433
        timeout = 5
        transport = "TCP"
      }

      startup_probe {
        failure_count_threshold = 10
        port = 1433
        timeout = 5
        transport = "TCP"
      }

    }
  }

  ingress {
    external_enabled = true
    exposed_port = (1433 + count.index)
    target_port = 1433 
    transport = "tcp"
    traffic_weight {
      percentage = 100
      latest_revision = true
    }
  }
}