/*
    Album <=> Genre (Influence)
*/

drop table if exists public.album_influence cascade;

create table public.album_influence (
    album_influence_album_id    bigint not null references public.albums,
    album_influence_genre_id    bigint not null references public.genres,
    primary key (album_influence_album_id, album_influence_genre_id)
);

alter table public.album_influence owner to postgres;

create index ix_albuminfluence_album_id on public.album_influence (album_influence_album_id);