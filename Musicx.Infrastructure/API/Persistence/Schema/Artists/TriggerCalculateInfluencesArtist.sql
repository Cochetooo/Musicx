CREATE OR REPLACE FUNCTION update_artist_calculated_influences(artistId BIGINT)
    RETURNS void AS
$$
WITH influence_counts AS (
    SELECT i.genre_name, COUNT(*) AS cnt
    FROM albums a
             JOIN album_influence ai ON ai.album_influence_album_id = a.album_id
             JOIN genres i ON i.genre_id = ai.album_influence_genre_id
    WHERE a.album_artist_id = artistId
    GROUP BY i.genre_name
    ORDER BY cnt DESC, i.genre_name
    LIMIT 5
)
UPDATE artists
SET artist_calculated_influences = (
    SELECT string_agg(genre_name, ' / ' ORDER BY cnt DESC, genre_name)
    FROM influence_counts
)
WHERE artist_id = artistId;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION on_album_influence_changed_artist()
    RETURNS trigger AS
$$
DECLARE
    affected_artist_id BIGINT;
BEGIN
    SELECT a.album_artist_id INTO affected_artist_id
    FROM albums a
    WHERE a.album_id = COALESCE(NEW.album_influence_album_id, OLD.album_influence_album_id);

    IF affected_artist_id IS NOT NULL THEN
        PERFORM update_artist_calculated_fields(affected_artist_id);
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trigger_update_artist_influences ON album_influence;

CREATE TRIGGER trigger_update_artist_influences
    AFTER INSERT OR UPDATE OR DELETE
    ON album_influence
    FOR EACH ROW
EXECUTE FUNCTION on_album_influence_changed_artist();