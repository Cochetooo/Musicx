/*
    BandArtist <=> PersonArtist
*/

drop table if exists public.bandartist_personartist cascade;

create table public.bandartist_personartist (
    bandartist_personartist_band_id   bigint not null references public.artists,
    bandartist_personartist_person_id bigint not null references public.artists,
    primary key (bandartist_personartist_band_id, bandartist_personartist_person_id)
);

alter table public.bandartist_personartist owner to postgres;

create index ix_bandartistpersonartist_person_id on public.bandartist_personartist (bandartist_personartist_person_id);