/*
    Song Audio Data 
*/

drop table if exists public.song_audio_data cascade;

create table public.song_audio_data (
    song_audio_data_song_id             bigint primary key references public.songs(song_id) on delete cascade,
    song_audio_data_bit_rate            integer,
    song_audio_data_file_path           text,
    song_audio_data_format              integer,
    song_audio_data_sample_rate         double precision,
    song_audio_data_volume_modifier     double precision
);

alter table public.song_audio_data owner to postgres;

