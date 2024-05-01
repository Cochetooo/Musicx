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

    List<AlbumDto> getAlbums();
    List<SongDto> getSongs();

    List<ArtistDto> findAll();
    List<ArtistDto> findByCriteria(final Map<ArtistSearchCriterias, Object> criteria);

    List<ArtistDto> fromSongs(final List<SongDto> songs);
    List<ArtistDto> fromAlbums(final List<AlbumDto> albums);
    List<ArtistDto> fromGenre(final GenreDto genre, final int mode);

    void create(final ArtistDto artist);

}
