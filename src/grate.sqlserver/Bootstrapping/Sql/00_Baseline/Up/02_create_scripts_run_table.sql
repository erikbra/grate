CREATE TABLE {{SchemaName}}.{{ScriptsRunTable}}(
    id bigint IDENTITY(1,1) NOT NULL,
    version_id BIGINT NULL,
    script_name nvarchar(255) NULL,
    text_of_script text NULL,
    text_hash nvarchar(512) NULL,
    one_time_script bit NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL,
    CONSTRAINT PK_{{ScriptsRunTable}}_id PRIMARY KEY CLUSTERED (id)
)
