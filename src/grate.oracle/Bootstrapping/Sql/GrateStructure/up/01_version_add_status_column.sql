DECLARE
    column_exists EXCEPTION;
    PRAGMA EXCEPTION_INIT (column_exists , -01430);
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE {{SchemaName}}_{{VersionTable}} ADD status VARCHAR2(50)';
EXCEPTION 
    WHEN column_exists THEN NULL;
END;