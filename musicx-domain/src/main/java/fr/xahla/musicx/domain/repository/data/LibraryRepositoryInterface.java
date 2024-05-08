package fr.xahla.musicx.domain.repository.data;

import fr.xahla.musicx.domain.model.data.LibraryInterface;
import fr.xahla.musicx.domain.repository.searchCriterias.LibrarySearchCriterias;

import java.util.List;

public interface LibraryRepositoryInterface {

    List<LibraryInterface> findByCriterias(final LibrarySearchCriterias ... criterias);

    void save(final LibraryInterface library);

}
