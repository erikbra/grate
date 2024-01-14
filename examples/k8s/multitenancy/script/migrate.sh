#!/bin/sh
# https://erikbra.github.io/grate/getting-started/
# https://erikbra.github.io/grate/configuration-options/
# databasetype <mariadb | oracle | postgresql | sqlite | sqlserver
for i in $(echo $Database__Databases | tr "," "\n")
do
  targetConnectionString=$(echo $APP_CONNSTRING | sed "s/{{targetDatabase}}/$i/g")
  echo "Migrating $targetConnectionString"
  ./grate --connectionstring "$targetConnectionString" \
        --adminconnectionstring "$ADMIN_CONNSTRING" \
        --sqlfilesdirectory /db \
        --environment ${Database__Env:-PROD} \
        --databasetype ${Database__Type:-sqlserver} \
        --silent \
        --transaction ${Database__Transaction:-true} || exit 1
done