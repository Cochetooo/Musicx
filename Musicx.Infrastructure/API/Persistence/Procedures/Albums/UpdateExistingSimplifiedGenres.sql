DO
$$
    DECLARE
        a_id INT;
    BEGIN
        FOR a_id IN
            SELECT DISTINCT ag.album_genre_album_id
            FROM album_genre ag
                     INNER JOIN albums al ON ag.album_genre_album_id=al.album_id
            LOOP
                PERFORM update_album_simplified_genre(a_id);
            END LOOP;
    END;
$$