package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.LibraryInterface;

public interface LibraryRepositoryInterface {

    /**
     * Clear all entries from the table.
     */
    void clear();

    /**
     * Find a library in the database with the corresponding id.
     */
    LibraryInterface findOneById(final Long id);

    /**
     * Find a library in the database with the corresponding name.
     */
    LibraryInterface findOneByName(final String name);

    /**
     * Save a library to the database.
     */
    void save(final LibraryInterface library);

}
