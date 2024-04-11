package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.ArtistInterface;
import javafx.beans.property.LongProperty;
import javafx.beans.property.SimpleLongProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

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
public class Artist implements ArtistInterface {

    private LongProperty id;
    private StringProperty name;
    private StringProperty country;

    public Artist() {
        this.id = new SimpleLongProperty();
        this.name = new SimpleStringProperty();
        this.country = new SimpleStringProperty();
    }

    public Artist set(ArtistInterface artist) {
        if (null != artist.getId()) {
            this.setId(artist.getId());
        }

        if (null != artist.getName()) {
            this.setName(artist.getName());
        }

        if (null != artist.getCountry()) {
            this.setCountry(artist.getCountry());
        }

        return this;
    }

    @Override public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    @Override public Artist setId(Long id) {
        this.id.set(id);
        return this;
    }

    @Override
    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Artist setName(String name) {
        this.name.set(name);
        return this;
    }

    @Override public String getCountry() {
        return country.get();
    }

    public StringProperty countryProperty() {
        return country;
    }

    @Override public Artist setCountry(String country) {
        this.country.set(country);
        return this;
    }
}
