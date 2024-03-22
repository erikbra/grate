CREATE TABLE {{SchemaName}}.{{ScriptsRunErrorsTable}}(
    id bigint IDENTITY(1,1) NOT NULL,
    repository_path nvarchar(255) NULL,
    version nvarchar(50) NULL,
    script_name nvarchar(255) NULL,
    text_of_script text NULL,
    erroneous_part_of_script text NULL,
    error_message text NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL,
    CONSTRAINT PK_{{ScriptsRunErrorsTable}}_id PRIMARY KEY CLUSTERED (id)
)

