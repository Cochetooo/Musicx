UPDATE albums a
SET
    album_total_duration = COALESCE(s.total_duration, 0),
    album_track_total   = COALESCE(s.track_count, 0),
    album_disc_total    = COALESCE(s.max_disc, 0)
FROM (
         SELECT
             song_album_id AS album_id,
             SUM(song_duration) AS total_duration,
             COUNT(*) AS track_count,
             MAX(song_disc_number) AS max_disc
         FROM songs
         GROUP BY song_album_id
     ) s
WHERE a.album_id = s.album_id;