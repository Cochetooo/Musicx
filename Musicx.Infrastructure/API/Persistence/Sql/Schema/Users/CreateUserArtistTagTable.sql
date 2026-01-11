/*
    Artist <= Tags => User
*/

drop table if exists public.user_artist_tags cascade;

create table public.user_artist_tags (
    user_artist_tags_user_id                     bigint references public.users(user_id) on delete cascade,
    user_artist_tags_artist_id                   bigint references public.artists(artist_id) on delete cascade,
    user_artist_tags_tag_id                      bigint references public.tags(tag_id) on delete cascade,
    user_artist_tags_created_at                  timestamp without time zone,
    primary key (user_artist_tags_user_id, user_artist_tags_artist_id, user_artist_tags_tag_id)
);

alter table public.user_artist_tags owner to postgres;

create index ix_uat_tag_artist on public.user_artist_tags (user_artist_tags_tag_id, user_artist_tags_artist_id);
create index ix_uat_artist_tag on public.user_artist_tags (user_artist_tags_artist_id, user_artist_tags_tag_id);
create index ix_uat_user_tag   on public.user_artist_tags (user_artist_tags_user_id, user_artist_tags_tag_id);
