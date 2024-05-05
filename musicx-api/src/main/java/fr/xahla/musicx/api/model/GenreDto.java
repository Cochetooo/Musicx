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

    private Long id;

    private List<Long> parentIds;

    private String name;

    public Long getId() {
        return id;
    }

    public GenreDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public GenreDto setName(final String name) {
        this.name = name;
        return this;
    }

    public List<Long> getParentIds() {
        return parentIds;
    }

    public GenreDto setParentIds(final List<Long> parentIds) {
        this.parentIds = parentIds;
        return this;
    }
}
