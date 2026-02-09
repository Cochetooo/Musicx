/*
    Band <=> Member
*/

drop table if exists public.band_member cascade;

create table public.band_member (
    band_member_band_id   bigint not null references public.artists,
    band_member_member_id bigint not null references public.artists,
    
    band_member_alias text,
    
    primary key (band_member_band_id, band_member_member_id)
);

alter table public.band_member owner to postgres;

create index ix_band_member_member_id on public.band_member (band_member_member_id);

\i 'Artists/CreateBandMemberPeriodTable.sql'