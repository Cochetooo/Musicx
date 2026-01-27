/*
    Song <= Attrs => User 
*/

drop table if exists public.user_song_attrs cascade;

create table public.user_song_attrs (
    user_song_attrs_user_id                   bigint references public.users(user_id) on delete cascade,
    user_song_attrs_song_id                   bigint references public.songs(song_id) on delete cascade,
    user_song_attrs_created_at                timestamp without time zone,
    user_song_attrs_updated_at                timestamp without time zone,
    user_song_attrs_attr                    smallint not null check (user_song_attrs_attr between 0 and 10000),
    primary key (user_song_attrs_user_id, user_song_attrs_song_id)
);

alter table public.user_song_attrs owner to postgres;

create index ix_usrat_song on public.user_song_attrs (user_song_attrs_song_id, user_song_attrs_attr);
create index ix_usrat_user on public.user_song_attrs (user_song_attrs_user_id);