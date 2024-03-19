-- We need to do 'if not exists' on this, as it is run twice, one for the Grate... tables,
-- and one for the "standard" migration tables
CREATE SCHEMA IF NOT EXISTS "{{SchemaName}}";