/*
    Song <=> Genre (Influence)
*/

drop table if exists public.song_influence cascade;

create table public.song_influence (
    song_influence_song_id      bigint not null references public.songs,
    song_influence_genre_id     bigint not null references public.genres,
    primary key (song_influence_song_id, song_influence_genre_id)
);

alter table public.song_influence owner to postgres;

create index ix_songinfluence_song_id on public.song_influence (song_influence_song_id);