CREATE TABLE {{SchemaName}}_{{ScriptsRunTable}}(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    version_id BIGINT NULL,
    script_name nvarchar(255) NULL,
    text_of_script ntext NULL,
    text_hash nvarchar(512) NULL,
    one_time_script bit NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL
)
