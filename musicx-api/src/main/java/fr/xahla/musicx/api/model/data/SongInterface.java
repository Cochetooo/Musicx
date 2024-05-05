package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.enums.AudioFormat;

import java.util.List;
import java.util.Map;

public interface SongInterface {

    /* --- DTO Cast --- */

    SongInterface fromDto(final SongDto albumDto);
    SongDto toDto();

    /* --- PRIMARY KEY --- */

    Long getId();
    SongInterface setId(final Long id);

    /* --- Album --- */

    AlbumInterface getAlbum();
    SongInterface setAlbum(final AlbumInterface album);

    /* --- Artist --- */

    ArtistInterface getArtist();
    SongInterface setArtist(final ArtistInterface artist);

    /* --- Bit rate --- */

    int getBitRate();
    SongInterface setBitRate(final int bitRate);

    /* --- Disc Number --- */

    short getDiscNumber();
    SongInterface setDiscNumber(final short discNumber);

    /* --- Duration --- */

    long getDuration();
    SongInterface setDuration(final long duration);

    /* --- Format --- */

    AudioFormat getFormat();
    SongInterface setFormat(final AudioFormat format);

    /* --- Lyrics --- */

    Map<Long, String> getLyrics();
    SongInterface setLyrics(final String lyrics);
    SongInterface setLyrics(final Map<Long, String> lyrics);

    /* --- Primary Genres --- */

    List<? extends GenreInterface> getPrimaryGenres();
    SongInterface setPrimaryGenres(final List<? extends GenreInterface> primaryGenres);

    /* --- Sample rate --- */

    int getSampleRate();
    SongInterface setSampleRate(final int sampleRate);

    /* --- Secondary Genres --- */

    List<? extends GenreInterface> getSecondaryGenres();
    SongInterface setSecondaryGenres(final List<? extends GenreInterface> secondaryGenres);

    /* --- Title --- */

    String getTitle();
    SongInterface setTitle(final String title);

    /* --- Track Number --- */

    short getTrackNumber();
    SongInterface setTrackNumber(final short trackNumber);

}
