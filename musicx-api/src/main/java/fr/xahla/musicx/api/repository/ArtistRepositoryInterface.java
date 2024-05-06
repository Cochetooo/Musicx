package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriterias;

import java.util.List;
import java.util.Map;

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

    List<AlbumDto> getAlbums(final ArtistDto artist);
    List<SongDto> getSongs(final ArtistDto artist);

    List<ArtistDto> findAll();
    List<ArtistDto> findByCriteria(final Map<ArtistSearchCriterias, Object> criteria);
    List<ArtistDto> findById(final Long id);

    void save(final ArtistDto artist);

}
