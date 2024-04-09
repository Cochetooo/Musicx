package fr.xahla.musicx.api.model;

public interface ArtistInterface {

    /* @var Long id */

    Long getId();
    ArtistInterface setId(final Long id);

    /* @var String name */

    String getName();
    ArtistInterface setName(final String name);

    /* @var String country */

    String getCountry();
    ArtistInterface setCountry(final String country);

    /* Hydrate data from another Artist */

    ArtistInterface set(final ArtistInterface artistInterface);

}
