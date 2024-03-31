CREATE TABLE {{SchemaName}}_{{ScriptsRunTable}}(
    id NUMBER(19) GENERATED ALWAYS AS IDENTITY NOT NULL PRIMARY KEY,
    version_id NUMBER(19) NULL,
    script_name VARCHAR2(255) NULL,
    text_of_script CLOB NULL,
    text_hash VARCHAR2(512) NULL,
    one_time_script CHAR(1) NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by VARCHAR2(50) NULL
)
