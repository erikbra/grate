DO LANGUAGE plpgsql
$$
BEGIN
    ALTER TABLE "{{SchemaName}}".{{ScriptsRunErrorsTable}} RENAME TO "{{ScriptsRunErrorsTable}}";
EXCEPTION WHEN undefined_table or duplicate_table THEN
END;
$$;