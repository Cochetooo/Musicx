package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.ArtistRole;
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

    ArtistDto getArtist(final AlbumDto album);
    Map<ArtistDto, ArtistRole> getCreditArtists(final AlbumDto album);
    LabelDto getLabel(final AlbumDto album);
    List<GenreDto> getPrimaryGenres(final AlbumDto album);
    List<GenreDto> getSecondaryGenres(final AlbumDto album);

    List<SongDto> getSongs(final AlbumDto album);

    AlbumDto find(final Long id);
    List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteria);
    List<AlbumDto> findAll();

    void save(final AlbumDto album);

}
