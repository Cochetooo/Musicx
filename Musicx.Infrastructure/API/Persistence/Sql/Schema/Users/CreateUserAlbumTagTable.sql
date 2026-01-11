/*
    Album <= Tags => User
*/

drop table if exists public.user_album_tags cascade;

create table public.user_album_tags (
    user_album_tags_user_id                     bigint references public.users(user_id) on delete cascade,
    user_album_tags_album_id                    bigint references public.albums(album_id) on delete cascade,
    user_album_tags_tag_id                      bigint references public.tags(tag_id) on delete cascade,
    user_album_tags_created_at                  timestamp without time zone,
    primary key (user_album_tags_user_id, user_album_tags_album_id, user_album_tags_tag_id)
);

alter table public.user_album_tags owner to postgres;

create index ix_uatg_tag_album on public.user_album_tags (user_album_tags_tag_id, user_album_tags_album_id);
create index ix_uatg_album_tag on public.user_album_tags (user_album_tags_album_id, user_album_tags_tag_id);
create index ix_uatg_user_tag  on public.user_album_tags (user_album_tags_user_id, user_album_tags_tag_id);
