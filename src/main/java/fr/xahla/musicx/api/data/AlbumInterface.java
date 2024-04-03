package fr.xahla.musicx.api.data;

public interface AlbumInterface {

    /* @var Long id */

    Long getId();
    AlbumInterface setId(Long id);

    /* @var ArtistInterface albumArtist */

    ArtistInterface getArtist();
    AlbumInterface setArtist(ArtistInterface artistInterface);

    /* @var String name */

    String getName();
    AlbumInterface setName(String name);

    /* @var Integer Year */

    Integer getReleaseYear();
    AlbumInterface setReleaseYear(Integer year);

}
