CREATE TABLE {{VersionTable}}(
    id bigint GENERATED ALWAYS AS IDENTITY NOT NULL
    repository_path varchar(255) NULL,
    version varchar(50) NULL,
    entry_date timestamp NULL,
    modified_date timestamp NULL,
    entered_by varchar(50) NULL
)



