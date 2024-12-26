CREATE TABLE {{SchemaName}}_{{ScriptsRunErrorsTable}}(
    id bigint NOT NULL AUTO_INCREMENT,
    repository_path varchar(255) NULL,
    version varchar(50) NULL,
    script_name varchar(255) NULL,
    text_of_script text NULL,
    erroneous_part_of_script text NULL,
    error_message text NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by varchar(50) NULL,
    CONSTRAINT PK_{{ScriptsRunErrorsTable}}_id PRIMARY KEY (id)
)