/*
    Favourite Album <=> User
*/

drop table if exists public.user_fav_album cascade;

create table public.user_fav_album (
    user_fav_album_user_id              bigint not null references public.users(user_id) on delete cascade,
    user_fav_album_album_id             bigint not null references public.albums(album_id) on delete cascade,
    
    user_fav_album_created_at           timestamp without time zone,
    user_fav_album_updated_at           timestamp without time zone,
    
    user_fav_album_note                 text,
    user_fav_album_order                smallint not null,
    
    primary key (user_fav_album_user_id, user_fav_album_album_id)
);

alter table public.user_fav_album owner to postgres;

create index ix_user_fav_album on public.user_fav_album (user_fav_album_user_id, user_fav_album_album_id);