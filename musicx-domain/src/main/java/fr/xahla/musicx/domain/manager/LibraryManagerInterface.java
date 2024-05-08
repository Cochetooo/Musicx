package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.domain.model.data.LibraryInterface;
import fr.xahla.musicx.domain.repository.data.LibraryRepositoryInterface;

public interface LibraryManagerInterface {

    LibraryInterface get();
    LibraryRepositoryInterface getRepository();

    void set(final LibraryInterface library);

}
