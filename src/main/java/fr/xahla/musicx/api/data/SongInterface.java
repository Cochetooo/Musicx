package fr.xahla.musicx.api.data;

public interface SongInterface {

    /* @var Long id */

    Long getId();
    SongInterface setId(Long id);

    /* @var ArtistInterface artist */

    ArtistInterface getArtist();
    SongInterface setArtist(ArtistInterface artistInterface);

    /* @var AlbumInterface album */

    AlbumInterface getAlbum();
    SongInterface setAlbum(AlbumInterface album);

    /* @var String title */

    String getTitle();
    SongInterface setTitle(String title);

    /* @var int duration */

    Integer getDuration();
    SongInterface setDuration(Integer duration);

    /* @var String filepath */

    String getFilePath();
    SongInterface setFilePath(String filePath);

    /* @var Integer bitRate */

    Integer getBitRate();
    SongInterface setBitRate(Integer bitRate);

    /* @var Integer sampleRate */

    Integer getSampleRate();
    SongInterface setSampleRate(Integer sampleRate);

}
