/*
    Album <=> Genre (Influence)
*/

drop table if exists public.album_influence cascade;

create table public.album_influence (
    album_influence_album_id        bigint not null references public.albums,
    album_influence_genre_id        bigint not null references public.genres,
    album_influence_tagger_id       bigint not null references public.users(user_id) on delete cascade,
    album_influence_created_at      timestamp without time zone,
    album_influence_updated_at      timestamp without time zone,
    album_influence_confidence      numeric(5,4) default 0.8 check (album_influence_confidence >= 0 and album_influence_confidence <= 1),
    album_influence_metadata        jsonb default '{}'::jsonb,
    album_influence_source          integer not null default 0,
    primary key (album_influence_album_id, album_influence_genre_id)
);

alter table public.album_influence owner to postgres;

create index ix_albuminfluence_album_id on public.album_influence (album_influence_album_id);