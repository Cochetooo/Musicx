package fr.xahla.musicx.api.model;

import java.util.List;

/** <b>(API) Interface for Song Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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

    /* @var Short trackNumber */

    Short getTrackNumber();
    SongInterface setTrackNumber(final Short trackNumber);

    /* @var Short discNumber */

    Short getDiscNumber();
    SongInterface setDiscNumber(final Short discNumber);

    /* @var GenreInterface[] primaryGenres */

    List<String> getPrimaryGenres();
    SongInterface setPrimaryGenres(final List<String> primaryGenres);

    /* @var GenreInterface[] secondaryGenres */

    List<String> getSecondaryGenres();
    SongInterface setSecondaryGenres(final List<String> secondaryGenres);

    /* Hydrate data from another Song */

    SongInterface set(final SongInterface songInterface);

}
