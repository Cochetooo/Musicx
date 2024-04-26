package fr.xahla.musicx.api.model;

import java.time.LocalDate;
import java.util.List;

/** <b>(API) Interface for Album Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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

    /* @var LocalDate releaseYear */

    LocalDate getReleaseDate();
    AlbumInterface setReleaseDate(final LocalDate year);

    /* @var Genre[] primaryGenres */

    List<GenreInterface> getPrimaryGenres();
    AlbumInterface setPrimaryGenres(final List<GenreInterface> genres);

    /* @var Genre[] secondaryGenres */

    List<GenreInterface> getSecondaryGenres();
    AlbumInterface setSecondaryGenres(final List<GenreInterface> genres);

    /* @var Short trackTotal */

    Short getTrackTotal();
    AlbumInterface setTrackTotal(final Short trackTotal);

    /* @var Short discTotal */

    Short getDiscTotal();
    AlbumInterface setDiscTotal(final Short discTotal);

    /* @var String artworkUrl */

    String getArtworkUrl();
    AlbumInterface setArtworkUrl(final String artworkUrl);

    /* Hydrate data from another Album */

    AlbumInterface set(final AlbumInterface albumInterface);

}
