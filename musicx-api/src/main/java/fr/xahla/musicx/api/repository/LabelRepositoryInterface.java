package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriterias;

import java.util.List;
import java.util.Map;

public interface LabelRepositoryInterface {

    List<AlbumDto> getReleases();

    List<LabelDto> findAll();
    List<LabelDto> findByCriteria(final Map<LabelSearchCriterias, Object> criteria);

    void save(final LabelDto label);

}
