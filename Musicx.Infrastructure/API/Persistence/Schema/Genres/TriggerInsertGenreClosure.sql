/*
 Auto Referencing Closure function
 */

CREATE OR REPLACE FUNCTION genre_insert_closure()
RETURNS trigger AS $$
BEGIN
    INSERT INTO genre_closure (genre_closure_ancestor_id, genre_closure_descendant_id, genre_closure_depth) 
    VALUES (NEW.genre_id, NEW.genre_id, 0);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_genre_insert_closure ON public.genres;

CREATE TRIGGER trg_genre_insert_closure
AFTER INSERT ON genres
    FOR EACH ROW EXECUTE FUNCTION genre_insert_closure();

/*
 Insert parent closure
 */

CREATE OR REPLACE FUNCTION genre_parent_insert_closure()
RETURNS trigger AS $$
BEGIN
    INSERT INTO genre_closure (genre_closure_ancestor_id, genre_closure_descendant_id, genre_closure_depth) 
    SELECT ap.genre_closure_ancestor_id, dc.genre_closure_descendant_id,
           ap.genre_closure_depth + dc.genre_closure_depth + 1
    FROM genre_closure ap
    CROSS JOIN genre_closure dc
    WHERE ap.genre_closure_descendant_id = NEW.genre_relation_from_genre_id
        AND dc.genre_closure_ancestor_id = NEW.genre_relation_to_genre_id
    ON CONFLICT DO NOTHING;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_genre_parent_insert_closure ON public.genre_relation;
CREATE TRIGGER trg_genre_parent_insert_closure
    AFTER INSERT ON genre_relation
FOR EACH ROW EXECUTE FUNCTION genre_parent_insert_closure();