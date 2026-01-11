/*
    User <=> Role 
*/

drop table if exists public.user_role cascade;

create table public.user_role (
    user_role_user_id bigint not null references public.users(user_id) on delete cascade,
    user_role_role_id smallint not null references public.roles(role_id) on delete cascade,
    primary key (user_role_user_id, user_role_role_id)
);

alter table public.user_role owner to postgres;