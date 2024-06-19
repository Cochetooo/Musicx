package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import javafx.beans.property.*;

import java.util.Locale;

/**
 * Defines the behaviour of an artist for a JavaFX context.
 * @author Cochetooo
 * @since 0.1.0
 */
public class Artist {

    protected final ArtistDto dto;

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

    /**
     * @since 0.3.0
     */
    public ArtistDto getDto() {
        return dto;
    }

    public long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Artist setId(final long id) {
        this.dto.setId(id);
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
        this.dto.setArtworkUrl(artworkUrl);
        this.artworkUrl.set(artworkUrl);
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Artist setName(final String name) {
        this.dto.setName(name);
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
        this.dto.setCountry(country);
        this.country.set(country);
        return this;
    }
}
