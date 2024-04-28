resource "random_password" "mariadb_admin" {
  length = 16
  special = true
}

resource "random_password" "mariadb_user" {
  length = 16
  special = true
}

resource "random_password" "oracle_admin" {
  length = 16
  special = false
}

resource "random_password" "postgresql_admin" {
  length = 16
  special = true
}

resource "random_password" "mssql_admin" {
  length = 16
  special = true
}

