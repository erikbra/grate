IF NOT EXISTS(
   SELECT 1 FROM sys.columns WHERE name = 'status'
   AND OBJECT_NAME(object_id) = '{{VersionTable}}')
BEGIN
    ALTER TABLE {{SchemaName}}.{{VersionTable}}
    ADD status nvarchar(50) NULL
END