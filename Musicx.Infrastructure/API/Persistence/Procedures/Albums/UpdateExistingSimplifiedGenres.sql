DO
$$
    DECLARE
        a_id INT;
    BEGIN
        FOR a_id IN
            SELECT DISTINCT ag.album_genre_album_id
            FROM album_genre ag
                     INNER JOIN albums al ON ag.album_genre_album_id=al.album_id
            WHERE
                al.album_simplified_genre_name IS NULL OR al.album_simplified_genre_name = ''
               OR al.album_simplified_genre_color IS NULL OR al.album_simplified_genre_color = ''
            LOOP
                PERFORM update_album_simplified_genre(a_id);
            END LOOP;
    END;
$$