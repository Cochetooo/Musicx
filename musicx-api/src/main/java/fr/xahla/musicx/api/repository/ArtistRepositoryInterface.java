package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.ArtistInterface;

/** <b>(API) Interface for Artist Repository contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface ArtistRepositoryInterface {

    ArtistInterface findById(final Integer id);

    void save(final ArtistInterface artist);

}
