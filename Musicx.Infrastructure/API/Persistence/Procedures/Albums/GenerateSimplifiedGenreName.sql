CREATE OR REPLACE FUNCTION update_album_simplified_genre(albumId INT)
    RETURNS void AS
$$
WITH raw_genres AS (
    SELECT g.genre_name, g.genre_color
    FROM album_genre ag
    JOIN genres g ON g.genre_id = ag.album_genre_genre_id
    WHERE ag.album_genre_album_id = albumId
),

-- Normalisation du nom (tirets -> espaces, qu'on réutilise partout)
normed AS (
 SELECT regexp_replace(genre_name, '-', ' ', 'g') AS norm_name, genre_color
 FROM raw_genres
),

-- ✅ Fix: si un seul mot, prefix_raw = '' et suffix = ce mot
split_genres AS (
 SELECT
     CASE
         WHEN norm_name LIKE '% %'
             THEN regexp_replace(norm_name, '\s+\S+$', '')
         ELSE ''                       -- <-- pas de préfixe pour un seul mot
         END AS prefix_raw,
     CASE
         WHEN norm_name LIKE '% %'
             THEN regexp_replace(norm_name, '^.*\s+', '')
         ELSE norm_name                -- <-- tout le mot = suffix
         END AS suffix,
     norm_name
 FROM normed
),

abbreviated AS (
 SELECT
     -- On abrège uniquement le 1er mot du préfixe (s’il existe)
     CASE
         WHEN prefix_raw = '' THEN ''  -- ✅ pas de préfixe => rien à abréger
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'progressive' THEN 'Prog'     || substr(prefix_raw, length('progressive') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'alternative' THEN 'Alt.'     || substr(prefix_raw, length('alternative') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'gothic'      THEN 'Goth'     || substr(prefix_raw, length('gothic') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'melodic'     THEN 'Melo'     || substr(prefix_raw, length('melodic') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'symphonic'   THEN 'Sympho'   || substr(prefix_raw, length('symphonic') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'electronic'  THEN 'Electro.' || substr(prefix_raw, length('electronic') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'industrial'  THEN 'Indus'    || substr(prefix_raw, length('industrial') + 1)
         WHEN LOWER(split_part(prefix_raw, ' ', 1)) = 'psychedelic' THEN 'Psych.'   || substr(prefix_raw, length('psychedelic') + 1)
         ELSE split_part(prefix_raw, ' ', 1) || substr(prefix_raw, length(split_part(prefix_raw, ' ', 1)) + 1)
         END AS abbr_prefix,
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

-- ✅ Fix: ne pas insérer d’espace si abbr_prefix = ''
format_by_prefix AS (
 SELECT
     CASE
         WHEN abbr_prefix = '' THEN joined_suffixes
         WHEN joined_suffixes IS NULL OR joined_suffixes = '' THEN abbr_prefix
         ELSE abbr_prefix || ' ' || joined_suffixes
         END AS simplified,
     joined_suffixes,
     abbr_prefix
 FROM grouped_by_prefix
),

-- Étape 2 : Alt. / Prog Metal
split_blocks AS (
 SELECT
     CASE
         WHEN simplified IS NULL OR simplified NOT LIKE '% %'
             THEN ''                                  -- un seul bloc => pas de préfixe
         ELSE regexp_replace(simplified, '\s+(\S+)$', '')
         END AS prefix_block,
     CASE
         WHEN simplified IS NULL OR simplified NOT LIKE '% %'
             THEN COALESCE(simplified, '')            -- tout est suffix
         ELSE regexp_replace(simplified, '^.*\s+', '')
         END AS suffix_block
 FROM format_by_prefix
),

-- ✅ Fix: ignorer les préfixes vides dans l’agrégation
grouped_by_suffix AS (
 SELECT
     suffix_block,
     string_agg(DISTINCT NULLIF(prefix_block, ''), ' / ' ORDER BY NULLIF(prefix_block, '')) AS joined_prefixes
 FROM split_blocks
 GROUP BY suffix_block
),

final_format AS (
 SELECT
     CASE
         WHEN COALESCE(joined_prefixes, '') = '' THEN suffix_block
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
        -- ✅ Distinct + trim par sûreté
        SELECT string_agg(DISTINCT btrim(simplified), ' / ' ORDER BY btrim(simplified))
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