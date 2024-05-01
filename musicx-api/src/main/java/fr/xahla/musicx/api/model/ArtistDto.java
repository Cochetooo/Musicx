package fr.xahla.musicx.api.model;

import java.util.Locale;

/** <b>(API) Interface for Artist Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public abstract class ArtistDto {

    private Long id;
    private String name;
    private Locale country;
    private String artworkUrl;

    public Long getId() {
        return id;
    }

    public ArtistDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public ArtistDto setName(final String name) {
        this.name = name;
        return this;
    }

    public Locale getCountry() {
        return country;
    }

    public ArtistDto setCountry(final Locale country) {
        this.country = country;
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl;
    }

    public ArtistDto setArtworkUrl(final String artworkUrl) {
        this.artworkUrl = artworkUrl;
        return this;
    }

}
