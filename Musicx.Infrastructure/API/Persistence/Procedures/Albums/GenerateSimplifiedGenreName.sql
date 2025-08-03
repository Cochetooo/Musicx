CREATE OR REPLACE FUNCTION update_album_simplified_genre(albumId INT)
    RETURNS void AS
$$
WITH raw_genres AS (
    SELECT g.genre_name, g.genre_color
    FROM album_genre ag
    JOIN genres g ON g.genre_id = ag.album_genre_genre_id
    WHERE ag.album_genre_album_id = albumId
),

split_genres AS (
    SELECT
        CASE
            WHEN strpos(genre_name, ' ') > 0 THEN split_part(genre_name, ' ', 1)
            ELSE genre_name
        END AS prefix,
        CASE
            WHEN strpos(genre_name, ' ') > 0 THEN split_part(genre_name, ' ', 2)
        END AS suffix
    FROM raw_genres
),
    
abbreviated AS (
    SELECT
        CASE
            WHEN LOWER(prefix) = 'progressive' THEN 'Prog'
            WHEN LOWER(prefix) = 'alternative' THEN 'Alt.'
            WHEN LOWER(prefix) = 'gothic' THEN 'Goth'
            WHEN LOWER(prefix) = 'melodic' THEN 'Melo'
            WHEN LOWER(prefix) = 'symphonic' THEN 'Sympho'
            WHEN LOWER(prefix) = 'electronic' THEN 'Electro.'
            WHEN LOWER(prefix) = 'industrial' THEN 'Indus'
            WHEN LOWER(prefix) = 'psychedelic' THEN 'Psych.'
            ELSE prefix
        END AS abbr_prefix,
        suffix
    FROM split_genres
),
    
grouped AS (
    SELECT
        abbr_prefix,
        string_agg(DISTINCT suffix, '/' ORDER BY suffix) AS joined_suffixes
    FROM abbreviated
    GROUP BY abbr_prefix
),
    
final_format AS (
    SELECT
        CASE
            WHEN joined_suffixes IS NULL THEN abbr_prefix
            ELSE abbr_prefix || ' ' || joined_suffixes
        END AS simplified
    FROM grouped
),
    
final_gradient AS (
    SELECT
        'linear-gradient(to left, ' || string_agg(COALESCE(genre_color, '#2b3333'), ', ') || ')' AS background
    FROM raw_genres
)

UPDATE albums
SET 
    album_simplified_genre_name = (
        SELECT string_agg(simplified, ' / ' ORDER BY simplified)
        FROM final_format
    ),
    album_simplified_genre_color = (
        SELECT background
        FROM final_gradient
    )
WHERE album_id = albumId
$$ LANGUAGE sql;

CREATE OR REPLACE FUNCTION on_album_genre_changed()
RETURNS trigger AS
$$
DECLARE
    affected_album_id INT;
BEGIN
    affected_album_id := COALESCE(NEW.album_genre_album_id, OLD.album_genre_album_id);
    PERFORM update_album_simplified_genre(affected_album_id);
    RETURN NULL;
end;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trigger_update_album_genre_name ON album_genre;

CREATE TRIGGER trigger_update_album_genre_name
AFTER INSERT OR UPDATE OR DELETE
ON album_genre
FOR EACH ROW
EXECUTE FUNCTION on_album_genre_changed();