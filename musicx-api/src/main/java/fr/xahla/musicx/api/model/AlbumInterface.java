package fr.xahla.musicx.api.model;

public interface AlbumInterface {

    /* @var Long id */

    Long getId();
    AlbumInterface setId(final Long id);

    /* @var ArtistInterface albumArtist */

    ArtistInterface getArtist();
    AlbumInterface setArtist(final ArtistInterface artistInterface);

    /* @var String name */

    String getName();
    AlbumInterface setName(final String name);

    /* @var Integer Year */

    Integer getReleaseYear();
    AlbumInterface setReleaseYear(final Integer year);

    /* Hydrate data from another Album */

    AlbumInterface set(final AlbumInterface albumInterface);

}
