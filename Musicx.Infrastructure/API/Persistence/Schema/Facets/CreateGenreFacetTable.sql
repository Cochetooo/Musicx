/*
    Genre <=> Facet
*/

drop table if exists public.genre_facet cascade;

create table public.genre_facet (
    genre_facet_genre_id            bigint not null references genres(genre_id) on delete cascade,
    genre_facet_facet_id            bigint not null references facets(facet_id) on delete cascade,
    genre_facet_created_at          timestamp without time zone,
    genre_facet_updated_at          timestamp without time zone,
    genre_facet_value               text not null,
    genre_facet_confidence          numeric(5,4) default 0.8 check (genre_facet_confidence >= 0 and genre_facet_confidence <= 1),
    genre_facet_metadata            jsonb default '{}'::jsonb,
    unique (genre_facet_genre_id, genre_facet_facet_id, genre_facet_value)
);

alter table public.genre_facet owner to postgres;

create index ix_genre_facet_genre on genre_facet (genre_facet_genre_id);