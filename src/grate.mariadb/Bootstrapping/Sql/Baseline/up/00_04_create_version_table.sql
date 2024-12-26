IF NOT EXISTS (SELECT 1 
                  FROM INFORMATION_SCHEMA.TABLES
                  WHERE table_schema = DATABASE() AND `table_name` =  '{{SchemaName}}_{{VersionTable}}'
            ) THEN
        CREATE TABLE {{SchemaName}}_{{VersionTable}}(
            id bigint NOT NULL AUTO_INCREMENT,
            repository_path varchar(255) NULL,
            version varchar(50) NULL,
            entry_date timestamp NULL,
            modified_date timestamp NULL,
            entered_by varchar(50) NULL,
            status varchar(50) NULL,
            CONSTRAINT PK_{{VersionTable}}_id PRIMARY KEY (id)
        );
END IF;
