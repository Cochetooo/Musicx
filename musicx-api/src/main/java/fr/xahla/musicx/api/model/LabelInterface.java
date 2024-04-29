package fr.xahla.musicx.api.model;

import java.util.List;

/** <b>(API) Interface for Label Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface LabelInterface {

    /* @var String name */

    String getName();
    LabelInterface setName(String name);

    /* @var GenreInterface[] genres */

    List<GenreInterface> getGenres();
    LabelInterface setGenres(List<GenreInterface> genres);

}
