/*
    Albums Ranking Stats 
*/

drop table if exists public.album_rating_stats cascade;

create table public.album_rating_stats (
    album_rating_stats_album_id             bigint primary key references public.albums(album_id) on delete cascade,
    album_rating_stats_count                integer not null default 0,
    album_rating_stats_sum                  bigint not null default 0,
    album_rating_stats_avg                  numeric(7,2) not null default 0
);