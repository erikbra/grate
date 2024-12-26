CREATE TABLE {{SchemaName}}_{{ScriptsRunTable}}(
    id bigint NOT NULL AUTO_INCREMENT,
    version_id BIGINT NULL,
    script_name varchar(255) NULL,
    text_of_script text NULL,
    text_hash varchar(512) NULL,
    one_time_script boolean NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by varchar(50) NULL,
    CONSTRAINT PK_{{ScriptsRunTable}}_id PRIMARY KEY (id)
)