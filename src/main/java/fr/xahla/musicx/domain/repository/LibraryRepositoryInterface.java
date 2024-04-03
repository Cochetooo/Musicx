package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.data.LibraryInterface;

public interface LibraryRepositoryInterface {

    void save(LibraryInterface library);
    void clear();
    LibraryInterface findOneByName(String name);
    LibraryInterface findOneById(Long id);

}
