DO LANGUAGE plpgsql
$$
BEGIN
    ALTER TABLE "{{SchemaName}}".{{ScriptsRunTable}} RENAME TO "{{ScriptsRunTable}}";
EXCEPTION WHEN undefined_table or duplicate_table THEN
END;
$$;