package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;

import java.util.List;

/** <b>(API) Interface for Album Repository contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface AlbumRepositoryInterface {

    void save(final AlbumInterface album);

    List<AlbumInterface> findByArtist(final ArtistInterface artistInterface);
    List<AlbumInterface> findByYear(final Integer year);

}
