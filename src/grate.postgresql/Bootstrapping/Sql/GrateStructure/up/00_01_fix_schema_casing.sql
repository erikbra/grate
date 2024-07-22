DO LANGUAGE plpgsql
$$
BEGIN
    ALTER SCHEMA {{SchemaName}} RENAME TO "{{SchemaName}}";
EXCEPTION WHEN duplicate_schema or invalid_schema_name THEN
END;
$$;