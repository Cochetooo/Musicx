package fr.xahla.musicx.core.model.data;

public interface LocalSongInterface {

    /**
     * Should be set to true if an error has occured while creating the local song.
     */
    boolean hasFailed();

    String getAlbumName();
    String getAlbumArtist();
    Integer getAlbumYear();
    String getArtistCountry();
    String getArtistName();
    Integer getBitRate();
    Short getBpm();
    String getCatalogNo();
    String getCoverArt();
    Short getDiscNumber();
    Short getDiscTotal();
    Integer getDuration();
    String getFormat();
    String getGenreName();
    String getLanguage();
    String getLyrics();
    Short getRating();
    String getRecordLabel();
    Integer getSampleRate();
    String getTitle();
    Short getTrackNumber();
    Short getTrackTotal();
    Integer getYear();

}
