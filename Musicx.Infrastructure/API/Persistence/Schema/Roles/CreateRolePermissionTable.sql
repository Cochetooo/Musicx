/*
    Role <=> Permissions
*/

drop table if exists public.role_permission cascade;

create table public.role_permission (
    role_permission_role_id         smallint not null references public.roles(role_id) on delete cascade,
    role_permission_permission_id   bigint not null references public.permissions(permission_id) on delete cascade,
    primary key (role_permission_role_id, role_permission_permission_id)
);

alter table public.role_permission owner to postgres;