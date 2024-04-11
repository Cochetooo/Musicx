package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.LibraryInterface;

/** <b>(API) Interface for Library Repository contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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
