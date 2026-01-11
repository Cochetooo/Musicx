/*
    Song <= Tags => User 
*/

drop table if exists public.user_song_tags cascade;

create table public.user_song_tags (
    user_song_tags_user_id                  bigint references public.users(user_id) on delete cascade,
    user_song_tags_song_id                  bigint references public.songs(song_id) on delete cascade,
    user_song_tags_tag_id                   bigint references public.tags(tag_id) on delete cascade,
    user_song_tags_created_at               timestamp without time zone,
    primary key (user_song_tags_user_id, user_song_tags_song_id, user_song_tags_tag_id)
);

alter table public.user_song_tags owner to postgres;

create index ix_ust_tag_song on public.user_song_tags (user_song_tags_tag_id, user_song_tags_song_id);
create index ix_ust_song_tag on public.user_song_tags (user_song_tags_song_id, user_song_tags_tag_id);
create index ix_ust_user_tag on public.user_song_tags (user_song_tags_user_id, user_song_tags_tag_id);