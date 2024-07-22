DO LANGUAGE plpgsql
$$
BEGIN
    ALTER TABLE "{{SchemaName}}".{{VersionTable}} RENAME TO "{{VersionTable}}";
EXCEPTION WHEN undefined_table or duplicate_table THEN
END;
$$;