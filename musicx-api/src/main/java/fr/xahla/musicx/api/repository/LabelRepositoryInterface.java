package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.LabelInterface;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriterias;

import java.util.List;

public interface LabelRepositoryInterface {

    List<AlbumInterface> getReleases();

    List<LabelInterface> findAll();
    List<LabelInterface> findByCriterias(final LabelSearchCriterias ... criterias);

    void save(final LabelInterface label);

}
