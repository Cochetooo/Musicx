/*
    Genre Relations 
*/

drop table if exists public.genre_relation cascade;

create table public.genre_relation (
    genre_relation_from_genre_id        bigint not null references genres(genre_id) on delete cascade,
    genre_relation_to_genre_id          bigint not null references genres(genre_id) on delete cascade,
    genre_relation_created_at           timestamp without time zone,
    genre_relation_updated_at           timestamp without time zone,
    genre_relation_metadata             jsonb default '{}'::jsonb,
    genre_relation_type                 integer not null,
    genre_relation_weight               numeric(5,4) default 1.0 CHECK (genre_relation_weight >= 0 AND genre_relation_weight <= 1),
    unique (genre_relation_from_genre_id, genre_relation_to_genre_id, genre_relation_type)
);

alter table public.genre_relation owner to postgres;

create index ix_genre_relation_from on genre_relation (genre_relation_from_genre_id);
create index ix_genre_relation_to on genre_relation (genre_relation_to_genre_id);