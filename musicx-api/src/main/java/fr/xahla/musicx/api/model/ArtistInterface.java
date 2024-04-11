package fr.xahla.musicx.api.model;

/** <b>(API) Interface for Artist Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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
