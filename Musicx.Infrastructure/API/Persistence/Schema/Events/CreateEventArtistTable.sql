/*
    Event <=> Artist 
*/

drop table if exists public.event_artist cascade;

create table public.event_artist (
    event_artist_event_id       bigint not null references public.events(event_id) on delete cascade,
    event_artist_artist_id      bigint not null references public.artists(artist_id) on delete cascade,
    event_artist_begin_date     timestamp without time zone,
    event_artist_end_date       timestamp without time zone
        check (event_artist_end_date is null or event_artist_end_date >= event_artist_begin_date)
);

alter table public.event_artist owner to postgres;
alter table public.event_artist add primary key (event_artist_event_id, event_artist_artist_id);