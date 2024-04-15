ALTER TABLE {{SchemaName}}.{{ScriptsRunErrorsTable}}
    ALTER COLUMN text_of_script NVARCHAR(MAX) NULL

ALTER TABLE {{SchemaName}}.{{ScriptsRunErrorsTable}}
    ALTER COLUMN erroneous_part_of_script NVARCHAR(MAX) NULL

ALTER TABLE {{SchemaName}}.{{ScriptsRunErrorsTable}}
    ALTER COLUMN error_message NVARCHAR(MAX) NULL
