CREATE OR REPLACE FUNCTION refresh_all_artists_calculated_fields()
    RETURNS void AS
$$
DECLARE
    r RECORD;
BEGIN
    FOR r IN SELECT artist_id FROM artists LOOP
            PERFORM update_artist_calculated_fields(r.artist_id);
        END LOOP;
END;
$$ LANGUAGE plpgsql;