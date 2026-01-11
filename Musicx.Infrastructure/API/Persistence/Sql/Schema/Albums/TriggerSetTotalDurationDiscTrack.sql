CREATE OR REPLACE FUNCTION update_album_fields()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE albums
    SET
        album_total_duration = COALESCE((
            SELECT SUM(song_duration)
            FROM songs
            WHERE song_album_id = NEW.song_album_id
        ), 0),
        album_track_total = COALESCE((
             SELECT COUNT(*)
             FROM songs
             WHERE song_album_id = NEW.song_album_id
         ), 0),
        album_disc_total = COALESCE((
            SELECT MAX(song_disc_number)
            FROM songs
            WHERE song_album_id = NEW.song_album_id
        ), 0)
    WHERE album_id = NEW.song_album_id;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_album_stats
    AFTER INSERT OR UPDATE OR DELETE ON songs
    FOR EACH ROW
    EXECUTE FUNCTION update_album_fields();