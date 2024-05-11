package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import javafx.beans.property.*;

import java.util.Locale;

/** <b>Class that defines the Artist Model for desktop view usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Artist {

    private final ArtistDto dto;

    private final LongProperty id;

    private final StringProperty artworkUrl;
    private final ObjectProperty<Locale> country;
    private final StringProperty name;

    public Artist(final ArtistDto artist) {
        this.id = new SimpleLongProperty(artist.getId());

        this.artworkUrl = new SimpleStringProperty(artist.getArtworkUrl());
        this.country = new SimpleObjectProperty<>(artist.getCountry());
        this.name = new SimpleStringProperty(artist.getName());

        this.dto = artist;
    }

    public ArtistDto toDto() {
        dto.setArtworkUrl(getArtworkUrl());
        dto.setCountry(getCountry());
        dto.setName(getName());

        return dto;
    }

    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Artist setId(final Long id) {
        this.id.set(id);
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl.get();
    }

    public StringProperty artworkUrlProperty() {
        return artworkUrl;
    }

    public void setArtworkUrl(final String artworkUrl) {
        this.artworkUrl.set(artworkUrl);
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Artist setName(final String name) {
        this.name.set(name);
        return this;
    }

    public Locale getCountry() {
        return country.get();
    }

    public ObjectProperty<Locale> countryProperty() {
        return country;
    }

    public Artist setCountry(final Locale country) {
        this.country.set(country);
        return this;
    }
}
