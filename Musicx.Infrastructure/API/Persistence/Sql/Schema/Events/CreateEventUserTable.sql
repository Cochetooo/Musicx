/*
    Event <=> User 
*/

drop table if exists public.event_user cascade;

create table public.event_user (
    event_user_event_id         bigint not null references public.events(event_id) on delete cascade,
    event_user_user_id          bigint not null references public.users(user_id) on delete cascade,
    event_user_comment          text,
    event_user_is_going         bool not null default false
);

alter table public.event_user owner to postgres;
alter table public.event_user add primary key (event_user_event_id, event_user_user_id);