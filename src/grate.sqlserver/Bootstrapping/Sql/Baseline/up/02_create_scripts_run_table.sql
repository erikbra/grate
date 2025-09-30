-- backwards compatibility, if the table already exists, let update scripts fix text column type issue
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'{{SchemaName}}.{{ScriptsRunTable}}') AND type in (N'U'))
    CREATE TABLE {{SchemaName}}.{{ScriptsRunTable}}(
        id bigint IDENTITY(1,1) NOT NULL,
        version_id BIGINT NULL,
        script_name nvarchar(255) NULL,
        text_of_script nvarchar(max) NULL,
        text_hash nvarchar(512) NULL,
        one_time_script bit NULL,
        entry_date datetime NULL,
        modified_date datetime NULL,
        entered_by nvarchar(50) NULL,
        CONSTRAINT PK_{{ScriptsRunTable}}_id PRIMARY KEY CLUSTERED (id)
    )