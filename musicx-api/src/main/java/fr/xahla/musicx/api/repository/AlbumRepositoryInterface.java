package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;

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

    List<SongInterface> getSongs();

    List<AlbumInterface> findByCriterias(final AlbumSearchCriterias ... criterias);
    List<AlbumInterface> findAll();

    List<AlbumInterface> fromSongs(final List<SongInterface> songs);
    List<AlbumInterface> fromArtist(final ArtistInterface artist);
    List<AlbumInterface> fromGenre(final GenreInterface genre, final int mode);

    void save(final AlbumInterface album);

}
