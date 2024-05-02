package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;

import java.util.List;
import java.util.Map;

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

    List<SongDto> getSongs(final AlbumDto album);

    List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteria);
    List<AlbumDto> findAll();

    List<AlbumDto> fromSongs(final List<SongDto> songs);

    void save(final AlbumDto album);

}
