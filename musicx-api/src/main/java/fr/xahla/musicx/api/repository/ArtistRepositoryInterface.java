package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriterias;

import java.util.List;

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

    List<AlbumInterface> getAlbums();
    List<SongInterface> getSongs();

    List<ArtistInterface> findAll();
    List<ArtistInterface> findByCriterias(final ArtistSearchCriterias ... criterias);

    List<ArtistInterface> fromSongs(final List<SongInterface> songs);
    List<ArtistInterface> fromAlbums(final List<AlbumInterface> albums);
    List<ArtistInterface> fromGenre(final GenreInterface genre, final int mode);

    void save(final ArtistInterface artist);

}
