/*
    Genre Closure 
*/

drop table if exists public.genre_closure cascade;

create table public.genre_closure (
    genre_closure_ancestor_id               bigint not null references genres(genre_id) on delete cascade,
    genre_closure_descendant_id             bigint not null references genres(genre_id) on delete cascade,
    genre_closure_depth                     int not null check (genre_closure_depth >= 0),
    primary key (genre_closure_ancestor_id, genre_closure_descendant_id)
);

alter table public.genre_closure owner to postgres;

create index ix_genre_closure_ancestor on genre_closure (genre_closure_ancestor_id);
create index ix_genre_closure_descendant on genre_closure (genre_closure_descendant_id);