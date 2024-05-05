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

    private Long id;

    private List<Long> genreIds;

    private String name;

    public Long getId() {
        return id;
    }

    public LabelDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public LabelDto setName(final String name) {
        this.name = name;
        return this;
    }

    public List<Long> getGenreIds() {
        return genreIds;
    }

    public LabelDto setGenreIds(final List<Long> genreIds) {
        this.genreIds = genreIds;
        return this;
    }
}
