ALTER TABLE {{SchemaName}}_{{VersionTable}}
ADD COLUMN IF NOT EXISTS status varchar(50) NULL;