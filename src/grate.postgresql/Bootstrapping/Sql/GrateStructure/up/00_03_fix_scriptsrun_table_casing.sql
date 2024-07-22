DO LANGUAGE plpgsql
$$
BEGIN
    ALTER TABLE "{{SchemaName}}"."{{ScriptsRunTableLowerCase}}" RENAME TO "{{ScriptsRunTable}}";
EXCEPTION WHEN undefined_table or duplicate_table THEN
END;
$$;