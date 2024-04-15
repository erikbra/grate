ALTER TABLE {{SchemaName}}_{{VersionTable}}
    MODIFY repository_path varchar(255) CHARACTER SET utf8mb4  NULL,
    MODIFY version varchar(50) CHARACTER SET utf8mb4  NULL,
    MODIFY entered_by varchar(50) CHARACTER SET utf8mb4  NULL,
    MODIFY status varchar(50) CHARACTER SET utf8mb4  NULL