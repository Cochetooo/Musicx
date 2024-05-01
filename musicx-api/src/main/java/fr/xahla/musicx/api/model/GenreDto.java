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
public class GenreDto {

    private String name;
    private List<GenreDto> parents;

    public String getName() {
        return name;
    }

    public GenreDto setName(final String name) {
        this.name = name;
        return this;
    }

    public List<GenreDto> getParents() {
        return parents;
    }

    public GenreDto setParents(final List<GenreDto> parents) {
        this.parents = parents;
        return this;
    }

}
