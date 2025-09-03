/*
    Song <= Ratings => User 
*/

drop table if exists public.user_song_ratings cascade;

create table public.user_song_ratings (
    user_song_ratings_user_id                   bigint references public.users(user_id) on delete cascade,
    user_song_ratings_song_id                   bigint references public.songs(song_id) on delete cascade,
    user_song_ratings_created_at                timestamp without time zone,
    user_song_ratings_updated_at                timestamp without time zone,
    user_song_ratings_rating                    smallint not null check (user_song_ratings_rating between 0 and 100),
    primary key (user_song_ratings_user_id, user_song_ratings_song_id)
);

alter table public.user_song_ratings owner to postgres;

create index ix_usr_song on public.user_song_ratings (user_song_ratings_song_id, user_song_ratings_rating);
create index ix_usr_user on public.user_song_ratings (user_song_ratings_user_id);