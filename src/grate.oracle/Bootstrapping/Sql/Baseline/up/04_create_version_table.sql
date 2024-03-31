CREATE TABLE {{SchemaName}}_{{VersionTable}}(
    id NUMBER(19) GENERATED ALWAYS AS IDENTITY NOT NULL PRIMARY KEY,
    repository_path VARCHAR2(255) NULL,
    version VARCHAR2(50) NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by VARCHAR2(50) NULL,
    status VARCHAR2(50) NULL
)
