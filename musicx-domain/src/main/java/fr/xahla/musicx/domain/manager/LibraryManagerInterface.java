package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.domain.model.LibraryInterface;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;

public interface LibraryManagerInterface {

    LibraryInterface get();
    LibraryRepositoryInterface getRepository();

    void set(final LibraryInterface library);

}
