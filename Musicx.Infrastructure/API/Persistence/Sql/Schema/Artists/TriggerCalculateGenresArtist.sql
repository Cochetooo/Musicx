/*
    Update Artist Genre Buckets
 */

CREATE OR REPLACE FUNCTION update_artist_calculated_genre_bucket(
    artistId BIGINT,
    targetColumn TEXT,
    sourceRelationTable TEXT,
    relationAlbumColumn TEXT,
    relationGenreColumn TEXT,
    genreTypes INTEGER[]
)
    RETURNS void AS
$$
DECLARE
    dynamicSql TEXT;
BEGIN
    dynamicSql := format($f$
                         WITH genre_counts AS (
                             SELECT
                                 g.genre_id,
                                 g.genre_canonical_name,
                                 COUNT(DISTINCT a.album_id)::int AS cnt
                             FROM albums a
                                      JOIN %1$I rel ON rel.%2$I = a.album_id
                                 JOIN genres g ON g.genre_id = rel.%3$I
                             WHERE a.album_artist_id = $1
                               AND g.genre_type = ANY($2)
                             GROUP BY g.genre_id, g.genre_canonical_name
                         )
                         UPDATE artists
                         SET %4$I = (
            SELECT COALESCE(
                           jsonb_agg(
                               jsonb_build_object(
                                   'genreId', genre_id,
                                   'count', cnt
                               )
                               ORDER BY cnt DESC, genre_canonical_name
                           )::text,
                           '[]'
                   )
            FROM genre_counts
        )
        WHERE artist_id = $1
    $f$, sourceRelationTable, relationAlbumColumn, relationGenreColumn, targetColumn);

    EXECUTE dynamicSql USING artistId, genreTypes;
END;
$$ LANGUAGE plpgsql;

/*
    Update Artist Genres Summary
 */

CREATE OR REPLACE FUNCTION update_artist_calculated_genres(artistId BIGINT)
    RETURNS void AS
$$
WITH genre_counts AS (
    SELECT
        g.genre_id,
        g.genre_canonical_name,
        COUNT(DISTINCT a.album_id)::int AS cnt
    FROM albums a
             JOIN album_genre ag ON ag.album_genre_album_id = a.album_id
             JOIN genres g ON g.genre_id = ag.album_genre_genre_id
    WHERE a.album_artist_id = artistId
      AND g.genre_type IN (0, 4, 5, 6)
    GROUP BY g.genre_id, g.genre_canonical_name
    ORDER BY cnt DESC, g.genre_canonical_name
)
UPDATE artists
SET artist_calculated_genres = (
    SELECT string_agg(genre_canonical_name, ' / ' ORDER BY cnt DESC, genre_canonical_name)
    FROM (
             SELECT genre_canonical_name, cnt
             FROM genre_counts
             LIMIT 3
         ) top_genres
),
artist_calculated_genre_counts = (
    SELECT COALESCE(
                   jsonb_agg(
                           jsonb_build_object(
                                   'genreId', genre_id,
                                   'count', cnt
                           )
                           ORDER BY cnt DESC, genre_canonical_name
                   )::text,
                   '[]'
           )
    FROM genre_counts
)
WHERE artist_id = artistId;
$$ LANGUAGE sql;

/*
    Update Artist Influences Summary
 */

CREATE OR REPLACE FUNCTION update_artist_calculated_influences(artistId BIGINT)
    RETURNS void AS
$$
WITH influence_counts AS (
    SELECT
        i.genre_id,
        i.genre_canonical_name,
        COUNT(DISTINCT a.album_id)::int AS cnt
    FROM albums a
             JOIN album_influence ai ON ai.album_influence_album_id = a.album_id
             JOIN genres i ON i.genre_id = ai.album_influence_genre_id
    WHERE a.album_artist_id = artistId
    GROUP BY i.genre_id, i.genre_canonical_name
    ORDER BY cnt DESC, i.genre_canonical_name
    LIMIT 5
)
UPDATE artists
SET artist_calculated_influences = (
    SELECT string_agg(genre_canonical_name, ' / ' ORDER BY cnt DESC, genre_canonical_name)
    FROM (
             SELECT genre_canonical_name, cnt
             FROM influence_counts
             LIMIT 5
         ) top_influences
),
artist_calculated_influence_counts = (
    SELECT COALESCE(
                   jsonb_agg(
                           jsonb_build_object(
                                   'genreId', genre_id,
                                   'count', cnt
                           )
                           ORDER BY cnt DESC, genre_canonical_name
                   )::text,
                   '[]'
           )
    FROM influence_counts
)
WHERE artist_id = artistId;
$$ LANGUAGE sql;

/*
  Update Artist Calculated Fields
 */

CREATE OR REPLACE FUNCTION update_artist_calculated_fields(artistId BIGINT)
    RETURNS void AS
$$
BEGIN
    PERFORM update_artist_calculated_genres(artistId);
    PERFORM update_artist_calculated_influences(artistId);
    PERFORM update_artist_calculated_genre_bucket(
            artistId,
            'artist_calculated_descriptor_counts',
            'album_genre',
            'album_genre_album_id',
            'album_genre_genre_id',
            ARRAY [1]
            );
    PERFORM update_artist_calculated_genre_bucket(
            artistId,
            'artist_calculated_scene_counts',
            'album_genre',
            'album_genre_album_id',
            'album_genre_genre_id',
            ARRAY [2]
            );
    PERFORM update_artist_calculated_genre_bucket(
            artistId,
            'artist_calculated_movement_counts',
            'album_genre',
            'album_genre_album_id',
            'album_genre_genre_id',
            ARRAY [3]
            );
END;
$$ LANGUAGE plpgsql;

/*
    Trigger Genres
 */

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

/*
    Trigger Influences
 */

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