CREATE TABLE {{SchemaName}}.{{VersionTable}}(
    id bigint IDENTITY(1,1) NOT NULL,
    repository_path nvarchar(255) NULL,
    version nvarchar(50) NULL,
    entry_date datetime NULL,
    modified_date datetime NULL,
    entered_by nvarchar(50) NULL,
    status nvarchar(50) NULL,
    CONSTRAINT PK_{{VersionTable}}_id PRIMARY KEY CLUSTERED (id)
)
