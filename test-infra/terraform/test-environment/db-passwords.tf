resource "random_password" "mariadb_admin" {
  length = 16
  special = true
}

resource "random_password" "mariadb_user" {
  length = 16
  special = true
}