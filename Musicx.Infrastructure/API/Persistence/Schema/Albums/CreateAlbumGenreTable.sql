/*
    Album <=> Genre (Primary)
*/

drop table if exists public.album_genre cascade;

create table public.album_genre (
    album_genre_album_id        bigint not null references public.albums,
    album_genre_genre_id        bigint not null references public.genres,
    primary key (album_genre_album_id, album_genre_genre_id)
);

alter table public.album_genre owner to postgres;

create index ix_albumgenre_album_id on public.album_genre (album_genre_album_id);