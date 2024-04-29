package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;

import java.util.List;

public interface GenreRepositoryInterface {

    List<GenreInterface> findByCriterias(final GenreSearchCriterias ... criterias);
    List<GenreInterface> findAll();

    List<GenreInterface> fromSongs(final List<SongInterface> songs);

    void save(final GenreInterface genre);

}
