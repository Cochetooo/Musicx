/*
    Album <= Attrs => User
*/

drop table if exists public.user_album_attrs cascade;

create table public.user_album_attrs (
    user_album_attrs_user_id                     bigint references public.users(user_id) on delete cascade,
    user_album_attrs_album_id                    bigint references public.albums(album_id) on delete cascade,
    user_album_attrs_created_at                  timestamp without time zone,
    user_album_attrs_updated_at                  timestamp without time zone,
    user_album_attrs_rating                      smallint check (user_album_attrs_rating between 0 and 10000),
    user_album_attrs_collection_type             integer,
    user_album_attrs_discovery_date              timestamp without time zone,
    user_album_attrs_review                      text,
    primary key (user_album_attrs_user_id, user_album_attrs_album_id)
);

alter table public.user_album_attrs owner to postgres;

create index ix_uaa_album on public.user_album_attrs (user_album_attrs_album_id);
create index ix_uaa_user on public.user_album_attrs (user_album_attrs_user_id);
create index ix_uaa_album_rating on public.user_album_attrs (user_album_attrs_album_id, user_album_attrs_rating) where user_album_attrs_rating is not null;
