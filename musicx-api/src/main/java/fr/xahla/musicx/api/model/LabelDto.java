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
public class LabelDto {

    private String name;
    private List<GenreDto> genres;

    public String getName() {
        return name;
    }

    public LabelDto setName(final String name) {
        this.name = name;
        return this;
    }

    public List<GenreDto> getGenres() {
        return genres;
    }

    public LabelDto setGenres(final List<GenreDto> genres) {
        this.genres = genres;
        return this;
    }

}
