ALTER TABLE "{{SchemaName}}"."{{VersionTable}}"
ADD COLUMN IF NOT EXISTS status nvarchar(50) NULL;