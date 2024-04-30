package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;

import java.util.List;

public interface GenreRepositoryInterface {

    List<? extends GenreInterface> findByCriterias(final GenreSearchCriterias ... criterias);
    List<? extends GenreInterface> findAll();

    List<? extends GenreInterface> fromSongs(final List<? extends SongInterface> songs);

    void save(final GenreInterface genre);

}
