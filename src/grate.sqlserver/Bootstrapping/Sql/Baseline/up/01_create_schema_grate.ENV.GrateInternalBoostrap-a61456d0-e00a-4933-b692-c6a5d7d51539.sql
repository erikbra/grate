-- We need to do 'if not exists' on this, as it is run twice, one for the Grate... tables,
-- and one for the "standard" migration tables
IF NOT EXISTS (SELECT *
 FROM sys.schemas
 WHERE name = N'{{SchemaName}}')
BEGIN
 EXEC('CREATE SCHEMA {{SchemaName}}')
END