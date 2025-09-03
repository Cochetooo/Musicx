/*
    Artist <= Ratings => User
*/

drop table if exists public.user_artist_ratings cascade;

create table public.user_artist_ratings(
    user_artist_ratings_user_id                     bigint references public.users(user_id) on delete cascade,
    user_artist_ratings_artist_id                   bigint references public.artists(artist_id) on delete cascade,
    user_artist_ratings_created_at                  timestamp without time zone,
    user_artist_ratings_updated_at                  timestamp without time zone,
    user_artist_ratings_rating                      smallint not null check (user_artist_ratings_rating between 0 and 100),
    primary key (user_artist_ratings_user_id, user_artist_ratings_artist_id)
);

alter table public.user_artist_ratings owner to postgres;

create index ix_uar_artist on public.user_artist_ratings (user_artist_ratings_artist_id, user_artist_ratings_rating);
create index ix_uar_user on public.user_artist_ratings (user_artist_ratings_user_id);