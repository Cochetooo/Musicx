/*
    Songs Ranking Stats 
*/

drop table if exists public.song_rating_stats cascade;

create table public.song_rating_stats (
    song_rating_stats_song_id               bigint primary key references public.songs(song_id) on delete cascade,
    song_rating_stats_count                 integer not null default 0,
    song_rating_stats_sum                   integer not null default 0,
    song_rating_stats_avg                   numeric(7,2) not null default 0
);

alter table public.song_rating_stats owner to postgres;