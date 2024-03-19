ALTER TABLE "{{SchemaName}}"."{{VersionTable}}"
ADD COLUMN IF NOT EXISTS status varchar(50) NULL;