CREATE TABLE {{SchemaName}}_{{ScriptsRunErrorsTable}}(
    id NUMBER(19) GENERATED ALWAYS AS IDENTITY NOT NULL PRIMARY KEY,
    repository_path VARCHAR2(255) NULL,
    version VARCHAR2(50) NULL,
    script_name VARCHAR2(255) NULL,
    text_of_script CLOB NULL,
    erroneous_part_of_script CLOB NULL,
    error_message CLOB NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by VARCHAR2(50) NULL
)

