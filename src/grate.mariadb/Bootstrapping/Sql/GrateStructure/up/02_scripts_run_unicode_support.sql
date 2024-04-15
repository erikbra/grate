ALTER TABLE {{SchemaName}}_{{ScriptsRunTable}}
    MODIFY script_name varchar(255) CHARACTER SET utf8mb4 NULL,
    MODIFY text_of_script text CHARACTER SET utf8mb4 NULL,
    MODIFY entered_by varchar(50) CHARACTER SET utf8mb4 NULL