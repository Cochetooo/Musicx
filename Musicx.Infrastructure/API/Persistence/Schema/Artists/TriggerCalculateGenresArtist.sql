CREATE OR REPLACE FUNCTION update_artist_calculated_genres(artistId BIGINT)
    RETURNS void AS
$$
WITH genre_counts AS (
    SELECT g.genre_name, COUNT(*) AS cnt
    FROM albums a
             JOIN album_genre ag ON ag.album_genre_album_id = a.album_id
             JOIN genres g ON g.genre_id = ag.album_genre_genre_id
    WHERE a.album_artist_id = artistId
    GROUP BY g.genre_name
    ORDER BY cnt DESC, g.genre_name
    LIMIT 3
)
UPDATE artists
SET artist_calculated_genres = (
    SELECT string_agg(genre_name, ' / ' ORDER BY cnt DESC, genre_name)
    FROM genre_counts
)
WHERE artist_id = artistId;
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION on_album_genre_changed_artist()
    RETURNS trigger AS
$$
DECLARE
    affected_artist_id BIGINT;
BEGIN
    SELECT a.album_artist_id INTO affected_artist_id
    FROM albums a
    WHERE a.album_id = COALESCE(NEW.album_genre_album_id, OLD.album_genre_album_id);

    IF affected_artist_id IS NOT NULL THEN
        PERFORM update_artist_calculated_fields(affected_artist_id);
    END IF;

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trigger_update_artist_genres ON album_genre;

CREATE TRIGGER trigger_update_artist_genres
    AFTER INSERT OR UPDATE OR DELETE
    ON album_genre
    FOR EACH ROW
EXECUTE FUNCTION on_album_genre_changed_artist();