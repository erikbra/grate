CREATE TABLE {{SchemaName}}_{{ScriptsRunErrorsTable}}(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    repository_path nvarchar(255) NULL,
    version nvarchar(50) NULL,
    script_name nvarchar(255) NULL,
    text_of_script ntext NULL,
    erroneous_part_of_script ntext NULL,
    error_message ntext NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL
)

