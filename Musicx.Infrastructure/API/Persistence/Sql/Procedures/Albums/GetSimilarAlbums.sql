DROP FUNCTION get_similar_albums(p_album_id bigint);

CREATE OR REPLACE FUNCTION get_similar_albums(
    p_album_id bigint
)
    RETURNS TABLE (
                      album_id bigint,
                      similarity_score double precision
                  )
    LANGUAGE sql
    STABLE
AS $$
WITH target_genres AS (
    SELECT
        ag.album_genre_genre_id AS genre_id
    FROM album_genre ag
    WHERE ag.album_genre_album_id = p_album_id
),

-- fréquence des genres (pour pondération type IDF)
     genre_df AS (
         SELECT
             album_genre_genre_id AS genre_id,
             COUNT(*) AS df
         FROM album_genre
         GROUP BY album_genre_genre_id
     ),

-- vecteur target pondéré
     target_vector AS (
         SELECT
             tg.genre_id,
             1.0 / LOG(2 + gd.df) AS weight
         FROM target_genres tg
                  JOIN genre_df gd ON gd.genre_id = tg.genre_id
     ),

-- norme du vecteur target
     target_norm AS (
         SELECT
             SQRT(SUM(weight * weight)) AS norm
         FROM target_vector
     ),

-- vecteurs candidats
     candidate_vectors AS (
         SELECT
             ag.album_genre_album_id AS album_id,
             ag.album_genre_genre_id AS genre_id,
             1.0 / LOG(2 + gd.df) AS weight
         FROM album_genre ag
                  JOIN genre_df gd ON gd.genre_id = ag.album_genre_genre_id
         WHERE ag.album_genre_album_id <> p_album_id
     ),

-- produit scalaire
     dot_products AS (
         SELECT
             cv.album_id,
             SUM(cv.weight * tv.weight) AS dot_product
         FROM candidate_vectors cv
                  JOIN target_vector tv ON tv.genre_id = cv.genre_id
         GROUP BY cv.album_id
     ),

-- norme des candidats
     candidate_norms AS (
         SELECT
             album_id,
             SQRT(SUM(weight * weight)) AS norm
         FROM candidate_vectors
         GROUP BY album_id
     ),

-- info artiste
     target_info AS (
         SELECT
             album_artist_id AS artist_id,
             EXTRACT(YEAR FROM album_original_release_date)::int AS year
         FROM albums
         WHERE album_id = p_album_id
     )

SELECT
    dp.album_id,
    (
        (dp.dot_product / (tn.norm * cn.norm))

            - CASE
                  WHEN al.album_artist_id = ti.artist_id THEN 0.15
                  ELSE 0
            END

            + (1.0 / (1 + ABS(
                EXTRACT(YEAR FROM al.album_original_release_date)::int - ti.year
                          ))) * 0.1

        ) AS similarity_score
FROM dot_products dp
         JOIN candidate_norms cn ON cn.album_id = dp.album_id
         JOIN albums al ON al.album_id = dp.album_id
         CROSS JOIN target_norm tn
         CROSS JOIN target_info ti
WHERE cn.norm > 0 AND tn.norm > 0
ORDER BY similarity_score DESC;
$$;