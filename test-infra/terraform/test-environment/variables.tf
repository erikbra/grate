# variable "MARIADB_ROOT_PASSWORD" {
#     sensitive = false
# }

locals {
  mariadb_images = [
    #"mariadb:5.5",
    #"mariadb:10.0",
    "mariadb:10.10"
   ]

  # Cannot get these to work yet, for some reason... need to look into it
   oracle_images = [
    "gvenzl/oracle-free:latest-faststart",
    "gvenzl/oracle-xe:latest-faststart",
    "gvenzl/oracle-xe:18-faststart",
    #"gvenzl/oracle-xe:11-faststart"
   ]

   postgresql_images = [
    "postgres:16",
    "postgres:15",
    "postgres:14",
    "postgres:13",
    "postgres:12"
   ]

   sqlserver_images = [
    #"mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04",
    "mcr.microsoft.com/mssql/server:2022-latest",
    "mcr.microsoft.com/mssql/server:2019-latest",
    "mcr.microsoft.com/mssql/server:2017-latest"
   ]
}