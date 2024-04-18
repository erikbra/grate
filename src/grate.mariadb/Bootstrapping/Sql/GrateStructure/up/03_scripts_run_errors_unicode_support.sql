ALTER TABLE {{SchemaName}}_{{ScriptsRunErrorsTable}}
    MODIFY repository_path varchar(255) CHARACTER SET utf8mb4 NULL,
    MODIFY version varchar(50) CHARACTER SET utf8mb4 NULL,
    MODIFY script_name varchar(255) CHARACTER SET utf8mb4 NULL,
    MODIFY text_of_script text CHARACTER SET utf8mb4 NULL,
    MODIFY erroneous_part_of_script text CHARACTER SET utf8mb4 NULL,
    MODIFY error_message text CHARACTER SET utf8mb4 NULL,
    MODIFY entered_by varchar(50) CHARACTER SET utf8mb4 NULL