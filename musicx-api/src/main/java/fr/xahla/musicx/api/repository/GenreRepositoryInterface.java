package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;

import java.util.List;
import java.util.Map;

public interface GenreRepositoryInterface {

    List<GenreDto> getParents(final GenreDto genre);

    GenreDto find(final Long id);
    List<GenreDto> findByCriteria(final Map<GenreSearchCriterias, Object> criteria);
    List<GenreDto> findAll();

    void save(final GenreDto genre);

}
