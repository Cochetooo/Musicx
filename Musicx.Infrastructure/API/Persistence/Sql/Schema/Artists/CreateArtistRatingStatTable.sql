/*
    Artists Ranking Stats 
*/

drop table if exists public.artist_rating_stats cascade;

create table public.artist_rating_stats (
    artist_rating_stats_artist_id           bigint primary key references public.artists(artist_id) on delete cascade,
    artist_rating_stats_count               integer not null default 0,
    artist_rating_stats_sum                 integer not null default 0,
    artist_rating_stats_avg                 numeric(5,2) not null default 0
);