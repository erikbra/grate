-- backwards compatibility, if the table already exists, let update scripts fix text column type issue
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{{SchemaName}}.{{ScriptsRunErrorsTable}}') AND type in (N'U'))
    CREATE TABLE {{SchemaName}}.{{ScriptsRunErrorsTable}}(
        id bigint IDENTITY(1,1) NOT NULL,
        repository_path nvarchar(255) NULL,
        version nvarchar(50) NULL,
        script_name nvarchar(255) NULL,
        text_of_script nvarchar(max) NULL,
        erroneous_part_of_script nvarchar(max) NULL,
        error_message nvarchar(max) NULL,
        entry_date datetime NULL,
        modified_date datetime NULL,
        entered_by nvarchar(50) NULL,
        CONSTRAINT PK_{{ScriptsRunErrorsTable}}_id PRIMARY KEY CLUSTERED (id)
    )