/*
    Album <=> Genre (Primary)
*/

drop table if exists public.album_genre cascade;

create table public.album_genre (
    album_genre_album_id        bigint not null references public.albums(album_id) on delete cascade,
    album_genre_genre_id        bigint not null references public.genres(genre_id) on delete cascade,
    album_genre_tagger_id       bigint not null references public.users(user_id) on delete cascade,
    album_genre_created_at      timestamp without time zone,
    album_genre_updated_at      timestamp without time zone,
    album_genre_confidence      numeric(5,4) not null default 0.8 
        check (album_genre_confidence >= 0 and album_genre_confidence <= 1),
    album_genre_metadata        jsonb default '{}'::jsonb,
    album_genre_source          integer not null default 0,
    primary key (album_genre_album_id, album_genre_genre_id, album_genre_tagger_id)
);

alter table public.album_genre owner to postgres;

create index ix_albumgenre_album_id on public.album_genre (album_genre_album_id);