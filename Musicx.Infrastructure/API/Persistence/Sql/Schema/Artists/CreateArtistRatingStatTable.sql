/*
    Artists Ranking Stats 
*/

drop table if exists public.artist_rating_stats cascade;

create table public.artist_rating_stats (
    artist_rating_stats_artist_id           bigint primary key references public.artists(artist_id) on delete cascade,
    artist_rating_stats_album_count         integer not null default 0,
    artist_rating_stats_count               bigint not null default 0,
    
    artist_rating_stats_sum                 bigint,
    
    artist_rating_stats_avg                 numeric(7,2),
    artist_rating_stats_variance            numeric(12,6),
    artist_rating_stats_std_dev             numeric(12,6),
    
    artist_rating_stats_skewness            numeric(12,6),
    artist_rating_stats_kurtosis            numeric(12,6),
    
    artist_rating_stats_bayesian_score      numeric(7,2),
    artist_rating_stats_legend_score        numeric(12,4)
);

alter table public.artist_rating_stats owner to postgres;

create index idx_artist_rating_stats_score ON artist_rating_stats(artist_rating_stats_bayesian_score DESC);
create index idx_artist_rating_stats_avg ON artist_rating_stats(artist_rating_stats_avg DESC);