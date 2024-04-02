output "mariadb-user-password" {
  value = random_password.mariadb_user.result
  sensitive = true
}


output "aca-env-public-ip" {
    value = azurerm_container_app_environment.grate-tests.static_ip_address
}

output "dbs" {
    value = concat(
      [
        for mariadb in azurerm_container_app.mariadb : { 
          name = mariadb.template[0].container[0].image
          type = "MariaDB"
          connectionstring= "Server=${mariadb.latest_revision_fqdn};Port=${mariadb.ingress[0].exposed_port};Database=mysql;Uid=root;Pwd='${random_password.mariadb_admin.result}'"
        }
      ],

      [
        for oracle in azurerm_container_app.oracle : { 
          name = "${replace(replace(oracle.template[0].container[0].image, "gvenzl/", ""), "-faststart", "")}"
          type = "Oracle"
          connectionstring= "Data Source=${oracle.latest_revision_fqdn}:${oracle.ingress[0].exposed_port}/FREEPDB1;User ID=system;Password='${random_password.oracle_admin.result}';Pooling=False"
        }
      ],

      [
        for postgres in azurerm_container_app.posgresql : { 
          name = postgres.template[0].container[0].image
          type = "PostgreSQL"
          connectionstring ="Host=${postgres.latest_revision_fqdn};Port=${postgres.ingress[0].exposed_port};Database=postgres;Username=postgres;Password='${random_password.postgresql_admin.result}';Include Error Detail=true;Pooling=false"
        }
      ],

      [
        for sqlserver in azurerm_container_app.sqlserver : { 
          name = "${replace(sqlserver.template[0].container[0].image, "mcr.microsoft.com/", "")}"
          type = "SqlServer"
          connectionstring = "Data Source=${sqlserver.latest_revision_fqdn},${sqlserver.ingress[0].exposed_port};Initial Catalog=master;User Id=sa;Password='${random_password.mssql_admin.result}';Encrypt=false;Pooling=false"
        }
      ]

    )
    sensitive = true
}