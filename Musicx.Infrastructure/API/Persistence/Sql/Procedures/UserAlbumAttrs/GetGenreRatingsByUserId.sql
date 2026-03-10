CREATE OR REPLACE FUNCTION get_genre_ratings_by_user_id(
    p_user_id bigint,
    p_skip bigint,
    p_take bigint
)
RETURNS TABLE (
    genre_id bigint,
    genre_name text,
    genre_color text,
    album_count bigint,
    weighted_score numeric,
    weighted_percent numeric
) LANGUAGE sql STABLE 
AS $$
    WITH album_scores AS (
        SELECT
            uaa.user_album_attrs_album_id AS album_id,
            ((uaa.user_album_attrs_rating - 5000.0) / 50.0) AS score
        FROM user_album_attrs uaa
        WHERE uaa.user_album_attrs_user_id = p_user_id
    ),

     user_avg AS (
         SELECT AVG(score) AS avg_score
         FROM album_scores
     ),

     genre_stats AS (
         SELECT
             g.genre_id,
             g.genre_canonical_name,
             g.genre_color,
             COUNT(*) AS album_count,
             SUM(s.score) AS weighted_score,
             AVG(s.score) AS avg_genre_score
         FROM album_scores s
                  JOIN album_genre ag ON s.album_id = ag.album_genre_album_id
                  JOIN genres g ON ag.album_genre_genre_id = g.genre_id
         GROUP BY g.genre_id, g.genre_canonical_name, g.genre_color
     ),

     prior AS (
         SELECT AVG(album_count)::float AS mean_count
         FROM genre_stats
     )
    
    SELECT
        gs.genre_id,
        gs.genre_canonical_name,
        gs.genre_color,
        gs.album_count,
        gs.weighted_score::numeric,

        (
            (gs.album_count * (gs.avg_genre_score - ua.avg_score))
                /
            (gs.album_count + p.mean_count)
        )::numeric AS weighted_percent

    FROM genre_stats gs
         CROSS JOIN user_avg ua
         CROSS JOIN prior p

    ORDER BY
        weighted_percent DESC,
        album_count DESC,
        genre_canonical_name

    OFFSET p_skip
    LIMIT p_take;
$$;