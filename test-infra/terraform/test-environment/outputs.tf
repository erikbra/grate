output "mariadb-user-password" {
  value = random_password.mariadb_user.result
  sensitive = true
}

output "mariadb-admin-password" {
  value = random_password.mariadb_admin.result
  sensitive = true
}


output "aca-env-public-ip" {
    value = azurerm_container_app_environment.grate-tests.static_ip_address
}

output "mariadb-fqdn" {
    value = azurerm_container_app.mariadb.latest_revision_fqdn
}