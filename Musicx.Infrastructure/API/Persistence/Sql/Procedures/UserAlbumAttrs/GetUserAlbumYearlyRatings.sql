CREATE OR REPLACE FUNCTION get_user_album_yearly_ratings(
    p_bucket_size INT DEFAULT 5,
    p_genre_id BIGINT DEFAULT NULL,
    p_user_id BIGINT DEFAULT NULL
)
    RETURNS TABLE(
                     year_bucket_start INT,
                     year_bucket_end INT,
                     year_bucket_label TEXT,
                     genre_id BIGINT,
                     genre_name TEXT,
                     genre_color TEXT,
                     average_rating NUMERIC,
                     rating_count INT
                 )
    LANGUAGE sql
AS $$
WITH base AS (
    SELECT
        u.user_album_attrs_rating AS rating,
        EXTRACT(YEAR FROM a.album_original_release_date)::INT AS release_year,
        ag.album_genre_genre_id AS genre_id,
        g.genre_canonical_name AS genre_name,
        g.genre_color AS genre_color,
        ROW_NUMBER() OVER (
            PARTITION BY u.user_album_attrs_album_id
            ORDER BY ag.album_genre_confidence DESC NULLS LAST,
                ag.album_genre_created_at ASC,
                ag.album_genre_genre_id ASC
            ) AS genre_rank
    FROM user_album_attrs u
             JOIN albums a
                  ON u.user_album_attrs_album_id = a.album_id
             LEFT JOIN album_genre ag
                       ON ag.album_genre_album_id = a.album_id
             LEFT JOIN genres g
                       ON g.genre_id = ag.album_genre_genre_id
    WHERE (p_user_id IS NULL OR u.user_album_attrs_user_id = p_user_id)
      AND u.user_album_attrs_rating IS NOT NULL
      AND a.album_original_release_date IS NOT NULL
),
     filtered AS (
         SELECT
             b.rating,
             b.release_year,
             b.genre_id,
             b.genre_name,
             b.genre_color,
             ((b.release_year / p_bucket_size) * p_bucket_size) AS year_bucket_start
         FROM base b
         WHERE (p_genre_id IS NULL AND b.genre_rank = 1)
            OR (p_genre_id IS NOT NULL AND b.genre_id = p_genre_id)
     )
SELECT
    f.year_bucket_start,
    f.year_bucket_start + p_bucket_size - 1 AS year_bucket_end,
    CONCAT(f.year_bucket_start, '-', f.year_bucket_start + p_bucket_size - 1) AS year_bucket_label,
    CASE WHEN p_genre_id IS NULL THEN NULL ELSE MAX(f.genre_id) END AS genre_id,
    CASE WHEN p_genre_id IS NULL THEN NULL ELSE MAX(f.genre_name) END AS genre_name,
    CASE WHEN p_genre_id IS NULL THEN NULL ELSE MAX(f.genre_color) END AS genre_color,
    ROUND(AVG(f.rating), 2) AS average_rating,
    COUNT(*)::INT AS rating_count
FROM filtered f
GROUP BY f.year_bucket_start
ORDER BY f.year_bucket_start;
$$;