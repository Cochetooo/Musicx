package fr.xahla.musicx.api.model;

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

    /* @var Long id */

    Long getId();
    GenreInterface setId(Long id);

    /* @var String name */

    String getName();
    GenreInterface setName(String name);

    /* @var GenreInterface parent */

    GenreInterface getParent();
    GenreInterface setParent(GenreInterface genre);

}
