/*
    Backfill existing users so that:
    - every user has pref_auto_follow = true
    - every user/artist pair inferred from an album rating >= 7000 is marked as follow = true
*/

update public.users
set user_pref_auto_follow = true,
    user_updated_at = now()
where coalesce(user_pref_auto_follow, false) = false;

insert into public.user_artist_attrs (
    user_artist_attrs_user_id,
    user_artist_attrs_artist_id,
    user_artist_attrs_created_at,
    user_artist_attrs_updated_at,
    user_artist_attrs_follow,
    user_artist_attrs_rating
)
select distinct
    uaa.user_album_attrs_user_id,
    al.album_artist_id,
    now(),
    now(),
    true,
    null::smallint
from public.user_album_attrs uaa
         join public.albums al
              on al.album_id = uaa.user_album_attrs_album_id
where uaa.user_album_attrs_rating >= 7000
  and al.album_artist_id is not null
on conflict (user_artist_attrs_user_id, user_artist_attrs_artist_id)
    do update set
                  user_artist_attrs_updated_at = excluded.user_artist_attrs_updated_at,
                  user_artist_attrs_follow = true;