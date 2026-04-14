CREATE OR REPLACE FUNCTION get_similar_albums(
    p_album_id bigint
)
    RETURNS TABLE (
        album_id bigint,
        similarity_score numeric
            )
    LANGUAGE sql
STABLE
AS $$
    WITH target_album AS (
        SELECT
            al0.album_id AS album_id,
            al0.album_artist_id AS artist_id,
            al0.album_original_release_date AS release_date,
            al0.album_simplified_genre_name AS simplified_genre_name
        FROM albums al0
        WHERE al0.album_id = p_album_id
    ),
    target_primary_genres AS (
        SELECT apg0.album_genre_genre_id AS genre_id
        FROM album_genre apg0
        WHERE apg0.album_genre_album_id = p_album_id
    ),
    target_influence_genres AS (
        SELECT aig0.album_influence_genre_id AS genre_id
        FROM album_influence aig0
        WHERE aig0.album_influence_album_id = p_album_id
    )
SELECT
    al1.album_id,
    (
        CASE WHEN al1.album_artist_id = ta.artist_id THEN 32 ELSE 0 END
            + COALESCE(ag_match.match_count, 0) * 18
            + COALESCE(ai_match.match_count, 0) * 12
            + CASE
                  WHEN al1.album_simplified_genre_name IS NOT NULL
                      AND al1.album_simplified_genre_name = ta.simplified_genre_name
                      THEN 6 ELSE 0 END
            + CASE
                  WHEN ar1.artist_calculated_scene_counts IS NOT NULL
                      AND ar0.artist_calculated_scene_counts IS NOT NULL
                      AND ar1.artist_calculated_scene_counts = ar0.artist_calculated_scene_counts
                      THEN 4 ELSE 0 END
            + CASE
                  WHEN ar1.artist_calculated_movement_counts IS NOT NULL
                      AND ar0.artist_calculated_movement_counts IS NOT NULL
                      AND ar1.artist_calculated_movement_counts = ar0.artist_calculated_movement_counts
                      THEN 4 ELSE 0 END
            + CASE
                  WHEN al1.album_original_release_date IS NOT NULL AND ta.release_date IS NOT NULL
                      THEN GREATEST(0, 10 - ABS(EXTRACT(YEAR FROM al1.album_original_release_date)::int - EXTRACT(YEAR FROM ta.release_date)::int))
                  ELSE 0 END
            + LOG(1 + COALESCE(ALST1.ALBUM_RATING_STATS_COUNT, 0)) * 2
        )::numeric AS similarity_score
FROM albums al1
         INNER JOIN target_album ta ON TRUE
         INNER JOIN artists ar1 ON ar1.artist_id = al1.album_artist_id
         INNER JOIN artists ar0 ON ar0.artist_id = ta.artist_id
         LEFT JOIN album_rating_stats alst1 ON alst1.album_rating_stats_album_id = al1.album_id
         LEFT JOIN LATERAL (
    SELECT COUNT(*) AS match_count
        FROM album_genre apg1
        WHERE apg1.album_genre_album_id = al1.album_id
            AND apg1.album_genre_genre_id IN (SELECT genre_id FROM target_primary_genres)
    ) ag_match ON TRUE
         LEFT JOIN LATERAL (
    SELECT COUNT(*) AS match_count
        FROM album_influence aig1
        WHERE aig1.album_influence_album_id = al1.album_id
            AND aig1.album_influence_genre_id IN (SELECT genre_id FROM target_influence_genres)
    ) ai_match ON TRUE
WHERE al1.album_id <> p_album_id
  AND al1.album_is_visible = TRUE;
$$;