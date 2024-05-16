package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriteria;

import java.util.List;
import java.util.Map;

/**
 * Contracts to manipulate genres in database.
 * @author Cochetooo
 * @see GenreDto
 * @since 0.3.0
 */
public interface GenreRepositoryInterface {

    List<GenreDto> getParents(final GenreDto genre);

    GenreDto find(final Long id);
    List<GenreDto> findByCriteria(final Map<GenreSearchCriteria, Object> criteria);
    List<GenreDto> findAll();

    void save(final GenreDto genre);

}
