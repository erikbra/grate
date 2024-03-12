locals {
  MARIADB_ROOT_PASSWORD = random_password.mariadb_admin.result
  MARIADB_USER_PASSWORD = random_password.mariadb_user.result
  MARIADB_USER = "mariadbuser"
}

resource "azurerm_container_app" "mariadb" {
  name                         = "mariadb-grate-tests"
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
      name          = "mariadb"
      image         = "mariadb:10.10"
      cpu           = 0.25
      memory        = "0.5Gi"

      # ref the MariaDB init script, here: https://github.com/MariaDB/mariadb-docker/blob/master/docker-entrypoint.sh#L377C2-L377C4
      env {
        name = "MARIADB_USER"
        value = local.MARIADB_USER
      }

      env {
        name = "MARIADB_PASSWORD" 
        value = local.MARIADB_USER_PASSWORD
      }

      env {
        name = "MARIADB_ROOT_HOST"
        #value = azurerm_container_app_environment.grate-tests.static_ip_address
        value = "%"
      }

      env {
        name = "MARIADB_ROOT_PASSWORD"
        value = local.MARIADB_ROOT_PASSWORD
      }

    }
  }
  ingress {
    external_enabled = true
    exposed_port = 3306
    target_port = 3306 
    transport = "tcp"
    traffic_weight {
      percentage = 100
      latest_revision = true
    }
  }
}