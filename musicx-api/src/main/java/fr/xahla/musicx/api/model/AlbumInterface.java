package fr.xahla.musicx.api.model;

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

    /* @var Integer Year */

    Integer getReleaseYear();
    AlbumInterface setReleaseYear(final Integer year);

    /* Hydrate data from another Album */

    AlbumInterface set(final AlbumInterface albumInterface);

}
