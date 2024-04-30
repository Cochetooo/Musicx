package fr.xahla.musicx.api.model;

import java.util.List;

/** <b>(API) Interface for Genre Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface GenreInterface {

    /* @var String name */

    String getName();
    GenreInterface setName(final String name);

    /* @var GenreInterface[] parents */

    List<? extends GenreInterface> getParents();
    GenreInterface setParents(final List<? extends GenreInterface> genre);

    /* GLOBAL SETTER */
    GenreInterface set(final GenreInterface genre);

}
