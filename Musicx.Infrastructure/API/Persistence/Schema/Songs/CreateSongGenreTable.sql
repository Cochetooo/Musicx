/*
    Song <=> Genre (Primary)
*/

drop table if exists public.song_genre cascade;

create table public.song_genre (
    song_genre_song_id          bigint not null references public.songs,
    song_genre_genre_id         bigint not null references public.genres,
    primary key (song_genre_song_id, song_genre_genre_id)
);

alter table public.song_genre owner to postgres;

create index ix_songgenre_song_id on public.song_genre (song_genre_song_id);