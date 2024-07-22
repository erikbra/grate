ALTER TABLE "{{SchemaName}}"."{{VersionTable}}"
ADD COLUMN IF NOT EXISTS status varchar(50) NULL;

-- DO LANGUAGE plpgsql
-- $$
-- BEGIN
--     ALTER TABLE "{{SchemaName}}"."{{VersionTable}}"
--     ADD COLUMN IF NOT EXISTS status varchar(50) NULL;
-- EXCEPTION WHEN OTHERS THEN
-- END;
-- $$;
-- 
-- DO LANGUAGE plpgsql
-- $$
-- BEGIN
-- ALTER TABLE {{SchemaName}}.{{VersionTable}}
--     ADD COLUMN IF NOT EXISTS status varchar(50) NULL;
-- EXCEPTION WHEN OTHERS THEN
-- END;
-- $$;