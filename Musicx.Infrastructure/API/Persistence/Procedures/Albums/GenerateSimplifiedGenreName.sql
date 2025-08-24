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
         regexp_replace(regexp_replace(genre_name, '-', ' '), '\s+(\S+)$', '') AS prefix_raw, -- préfixe brut (avec tiret → espace)
         regexp_replace(regexp_replace(genre_name, '-', ' '), '^.*\s+', '') AS suffix,       -- suffix = dernier mot
         genre_name
     FROM raw_genres
 ),

 abbreviated AS (
     SELECT
         -- on abrège uniquement le premier mot du préfixe
         CASE
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'progressive' THEN 'Prog'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'alternative' THEN 'Alt.'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'gothic' THEN 'Goth'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'melodic' THEN 'Melo'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'symphonic' THEN 'Sympho'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'electronic' THEN 'Electro.'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'industrial' THEN 'Indus'
             WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'psychedelic' THEN 'Psych.'
             ELSE split_part(prefix_raw, ' ', 1)
         END
         || substr(prefix_raw, length(split_part(prefix_raw, ' ', 1)) + 1) AS abbr_prefix, -- rajoute le reste
         suffix
     FROM split_genres
 ),

 -- Étape 1 : regroupement par préfixe => Alt. Rock/Metal
 grouped_by_prefix AS (
     SELECT
         abbr_prefix,
         string_agg(DISTINCT suffix, '/' ORDER BY suffix) AS joined_suffixes
     FROM abbreviated
     GROUP BY abbr_prefix
 ),

 format_by_prefix AS (
     SELECT
         CASE
             WHEN joined_suffixes IS NULL THEN abbr_prefix
             ELSE abbr_prefix || ' ' || joined_suffixes
             END AS simplified,
         joined_suffixes,
         abbr_prefix
     FROM grouped_by_prefix
 ),

-- Étape 2 : regroupement par suffixe => Alt. / Prog Metal
 split_blocks AS (
     SELECT
         CASE
             WHEN simplified NOT LIKE '% %' THEN ''  -- un seul mot => pas de prefix
             ELSE regexp_replace(simplified, '\s+(\S+)$', '')
             END AS prefix_block,
         CASE
             WHEN simplified NOT LIKE '% %' THEN simplified  -- un seul mot => tout est suffix
             ELSE regexp_replace(simplified, '^.*\s+', '')
             END AS suffix_block
     FROM format_by_prefix
 ),

 grouped_by_suffix AS (
     SELECT
         suffix_block,
         string_agg(DISTINCT prefix_block, ' / ' ORDER BY prefix_block) AS joined_prefixes
     FROM split_blocks
     GROUP BY suffix_block
 ),

 final_format AS (
     SELECT
         CASE
             WHEN joined_prefixes = '' THEN suffix_block
             ELSE joined_prefixes || ' ' || suffix_block
             END AS simplified
     FROM grouped_by_suffix
 ),
    
final_gradient AS (
    SELECT
        'linear-gradient(to left, ' || string_agg(COALESCE(genre_color, '#2b3333'), ', ') || ')' AS background
    FROM raw_genres
)

UPDATE albums
SET 
    album_simplified_genre_name = (
        SELECT regexp_replace(string_agg(simplified, ' / ' ORDER BY simplified),
                              '\b(\w+)\s+\1\b', '\1', 'gi')
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