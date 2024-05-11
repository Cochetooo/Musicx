package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.*;
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

    List<PersonArtistDto> getMembers(final BandArtistDto artist);
    List<BandArtistDto> getBands(final PersonArtistDto artist);

    List<AlbumDto> getAlbums(final ArtistDto artist);
    List<SongDto> getSongs(final ArtistDto artist);

    ArtistDto find(final Long id);
    List<ArtistDto> findAll();
    List<ArtistDto> findByCriteria(final Map<ArtistSearchCriterias, Object> criteria);

    void save(final ArtistDto artist);

}
