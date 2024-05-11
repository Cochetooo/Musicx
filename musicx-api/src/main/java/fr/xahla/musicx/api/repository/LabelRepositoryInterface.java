package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriterias;

import java.util.List;
import java.util.Map;

public interface LabelRepositoryInterface {

    List<GenreDto> getGenres(final LabelDto label);
    List<AlbumDto> getReleases(final LabelDto label);

    LabelDto find(final Long id);
    List<LabelDto> findAll();
    List<LabelDto> findByCriteria(final Map<LabelSearchCriterias, Object> criteria);

    void save(final LabelDto label);

}
