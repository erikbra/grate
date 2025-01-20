IF EXISTS (SELECT 1 
                  FROM INFORMATION_SCHEMA.TABLES
                  WHERE table_schema = DATABASE()
                    AND BINARY `table_name` = BINARY '{{SchemaName}}_{{ScriptsRunErrorsTableLowerCase}}' -- exists in db
      )
THEN
      -- we need to make sure the target table is not existing, otherwise, mariadb will throw an error like table already exists
      IF NOT EXISTS (SELECT 1
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE table_schema = DATABASE()
                  AND BINARY `table_name` = BINARY '{{SchemaName}}_{{ScriptsRunErrorsTable}}'
      )
      THEN
        RENAME TABLE `{{SchemaName}}_{{ScriptsRunErrorsTableLowerCase}}` TO `{{SchemaName}}_{{ScriptsRunErrorsTable}}`;
      END IF;
END IF;
