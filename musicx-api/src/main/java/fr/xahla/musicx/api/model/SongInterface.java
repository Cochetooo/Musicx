package fr.xahla.musicx.api.model;

public interface SongInterface {

    /* @var Long id */

    Long getId();
    SongInterface setId(final Long id);

    /* @var ArtistInterface artist */

    ArtistInterface getArtist();
    SongInterface setArtist(final ArtistInterface artistInterface);

    /* @var AlbumInterface album */

    AlbumInterface getAlbum();
    SongInterface setAlbum(final AlbumInterface album);

    /* @var String title */

    String getTitle();
    SongInterface setTitle(final String title);

    /* @var int duration */

    Integer getDuration();
    SongInterface setDuration(final Integer duration);

    /* @var String filepath */

    String getFilePath();
    SongInterface setFilePath(final String filePath);

    /* @var Boolean available */

    Boolean isAvailable();
    SongInterface setAvailable(final Boolean available);

    /* @var Integer bitRate */

    Integer getBitRate();
    SongInterface setBitRate(final Integer bitRate);

    /* @var Integer sampleRate */

    Integer getSampleRate();
    SongInterface setSampleRate(final Integer sampleRate);

    /* Hydrate data from another Song */

    SongInterface set(final SongInterface songInterface);

}
