-- The MariaDB version below is more elegant, but it is not compatible with MySQL.
-- ALTER TABLE {{SchemaName}}_{{VersionTable}}
-- ADD COLUMN IF NOT EXISTS status varchar(50) NULL;;

-- MySQL does not support the IF NOT EXISTS clause for ADD COLUMN, as MariaDB does.
-- So, instead of the MariaDB version, we need to use a handler to ignore the error if the column already exists.
-- This is a bit of a hack, but it works.
-- The MariaDB version is more elegant, but it is not compatible with MySQL.
CREATE PROCEDURE create_{{SchemaName}}_{{VersionTable}}()
BEGIN
    DECLARE CONTINUE HANDLER FOR 1060 BEGIN END;
    ALTER TABLE {{SchemaName}}_{{VersionTable}}
    ADD COLUMN status varchar(50) NULL;
END;
CALL create_{{SchemaName}}_{{VersionTable}}();

DROP PROCEDURE create_{{SchemaName}}_{{VersionTable}}
