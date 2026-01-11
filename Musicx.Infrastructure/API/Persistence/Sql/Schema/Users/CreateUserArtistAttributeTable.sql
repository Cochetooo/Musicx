/*
    Artist <= Attributes => User
*/

drop table if exists public.user_artist_attrs cascade;

create table public.user_artist_attrs(
    user_artist_attrs_user_id                     bigint references public.users(user_id) on delete cascade,
    user_artist_attrs_artist_id                   bigint references public.artists(artist_id) on delete cascade,
    user_artist_attrs_created_at                  timestamp without time zone,
    user_artist_attrs_updated_at                  timestamp without time zone,
    user_artist_attrs_follow                      bool,
    user_artist_attrs_rating                      smallint check (user_artist_attrs_rating between 0 and 100),
    primary key (user_artist_attrs_user_id, user_artist_attrs_artist_id)
);

alter table public.user_artist_attrs owner to postgres;

create index ix_uar_artist on public.user_artist_attrs (user_artist_attrs_artist_id, user_artist_attrs_rating);
create index ix_uar_user on public.user_artist_attrs (user_artist_attrs_user_id);