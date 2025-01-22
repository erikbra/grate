CREATE PROCEDURE create_{{SchemaName}}_fix_scriptsrun_table_casing()
BEGIN
    DECLARE CONTINUE HANDLER FOR 1060 BEGIN END;
    IF EXISTS (SELECT 1 
      FROM INFORMATION_SCHEMA.TABLES
      WHERE table_schema = DATABASE()
        AND BINARY `table_name` = BINARY '{{SchemaName}}_{{ScriptsRunTableLowerCase}}' -- exists in db
    )
    THEN
        -- we need to make sure the target table is not existing, otherwise, mariadb will throw an error like table already exists
        IF NOT EXISTS (SELECT 1
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE table_schema = DATABASE()
            AND BINARY `table_name` = BINARY '{{SchemaName}}_{{ScriptsRunTable}}'
        )
        THEN
            RENAME TABLE `{{SchemaName}}_{{ScriptsRunTableLowerCase}}` TO `{{SchemaName}}_{{ScriptsRunTable}}`;
        END IF;
    END IF;
END;
