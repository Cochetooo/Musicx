/*
    ChildrenGenre <=> ParentGenre
*/

drop table if exists public.childrengenre_parentgenre cascade;

create table public.childrengenre_parentgenre (
    childrengenre_parentgenre_child_id bigint not null references public.genres,
    childrengenre_parentgenre_parent_id   bigint not null references public.genres,
    primary key (childrengenre_parentgenre_child_id, childrengenre_parentgenre_parent_id)
);

alter table public.childrengenre_parentgenre owner to postgres;

create index ix_childrengenreparentgenre_parent_id on public.childrengenre_parentgenre (childrengenre_parentgenre_parent_id);