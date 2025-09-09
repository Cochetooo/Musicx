-- Truncate all tables
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
        EXECUTE 'TRUNCATE TABLE public.' || quote_ident(r.tablename) || ' CASCADE;';
    END LOOP;
END $$;

-- Insert fixtures
\i '_Fixtures/artists.sql'
\i '_Fixtures/genres.sql'
\i '_Fixtures/albums.sql'
\i '_Fixtures/songs.sql'

\i '_Fixtures/permissions.sql'
\i '_Fixtures/roles.sql'
\i '_Fixtures/users.sql'

\i '_Fixtures/childrengenre_parentgenre.sql'
\i '_Fixtures/album_genre.sql'
\i '_Fixtures/album_influence.sql'
\i '_Fixtures/song_genre.sql'
\i '_Fixtures/song_influence.sql'

-- Setup sequences
DO $$
DECLARE
    r RECORD;
    max_id bigint;
BEGIN
    FOR r IN
        SELECT
            ns.nspname              AS schema_name,
            tbl.relname             AS table_name,
            col.attname             AS column_name,
            seq.relname             AS sequence_name
        FROM pg_class seq
                 JOIN pg_namespace ns ON ns.oid = seq.relnamespace
                 JOIN pg_depend dep    ON dep.objid = seq.oid AND dep.deptype = 'a' -- 'a' = owned by column
                 JOIN pg_class tbl     ON tbl.oid = dep.refobjid
                 JOIN pg_attribute col ON col.attrelid = tbl.oid AND col.attnum = dep.refobjsubid
        WHERE seq.relkind = 'S'
          AND ns.nspname = 'public'
    LOOP
        -- max(id) de la table/colonne possédée par la séquence
        EXECUTE format('SELECT COALESCE(MAX(%I), 0) FROM %I.%I',
                       r.column_name, r.schema_name, r.table_name)
            INTO max_id;

        -- Positionne la séquence sur max(id) (prochain nextval() => max+1)
        EXECUTE format(
                'SELECT setval(%L::regclass, %s, true);',
                r.schema_name || '.' || r.sequence_name,
                max_id
                );
    END LOOP;
END $$;