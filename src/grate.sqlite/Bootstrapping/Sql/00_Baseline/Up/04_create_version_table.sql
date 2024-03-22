CREATE TABLE {{SchemaName}}_{{VersionTable}}(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    repository_path nvarchar(255) NULL,
    version nvarchar(50) NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL,
    status nvarchar(50) NULL
)
