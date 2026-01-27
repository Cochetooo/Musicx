ALTER TABLE album_rating_stats
    ALTER COLUMN album_rating_stats_avg TYPE numeric(7,2);

ALTER TABLE public.user_album_attrs
    DISABLE TRIGGER t_album_rating_stats;

BEGIN;

ALTER TABLE public.user_album_attrs
    DROP CONSTRAINT IF EXISTS user_album_attrs_user_album_attrs_rating_check;

UPDATE public.user_album_attrs
SET user_album_attrs_rating = user_album_attrs_rating * 100
WHERE user_album_attrs_rating IS NOT NULL;

ALTER TABLE public.user_album_attrs
    ADD CONSTRAINT user_album_attrs_rating_check
        CHECK (user_album_attrs_rating BETWEEN 0 AND 10000);

COMMIT;

UPDATE album_rating_stats s
SET
    album_rating_stats_count = sub.cnt,
    album_rating_stats_sum   = sub.sum,
    album_rating_stats_avg   = round(sub.sum::numeric / nullif(sub.cnt,0), 2)
FROM (
         SELECT
             user_album_attrs_album_id,
             count(user_album_attrs_rating) as cnt,
             sum(user_album_attrs_rating)   as sum
         FROM user_album_attrs
         WHERE user_album_attrs_rating IS NOT NULL
         GROUP BY user_album_attrs_album_id
     ) sub
WHERE s.album_rating_stats_album_id = sub.user_album_attrs_album_id;

ALTER TABLE public.user_album_attrs
    ENABLE TRIGGER t_album_rating_stats;