package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriteria;

import java.util.List;
import java.util.Map;

/**
 * Contracts to manipulate labels in database.
 * @author Cochetooo
 * @see LabelDto
 * @since 0.3.0
 */
public interface LabelRepositoryInterface {

    List<GenreDto> getGenres(final LabelDto label);
    List<AlbumDto> getReleases(final LabelDto label);

    LabelDto find(final Long id);
    List<LabelDto> findAll();
    List<LabelDto> findByCriteria(final Map<LabelSearchCriteria, Object> criteria);

    void save(final LabelDto label);

}
