package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import javafx.beans.property.*;

/** <b>Class that defines the Album Model for desktop view usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Album implements AlbumDto {

    private final LongProperty id;
    private final StringProperty name;
    private final IntegerProperty releaseYear;
    private final IntegerProperty trackTotal;
    private final IntegerProperty discTotal;
    private final ObjectProperty<Artist> artist;

    public Album() {
        this.id = new SimpleLongProperty();
        this.name = new SimpleStringProperty();
        this.releaseYear = new SimpleIntegerProperty();
        this.trackTotal = new SimpleIntegerProperty();
        this.discTotal = new SimpleIntegerProperty();
        this.artist = new SimpleObjectProperty<>(new Artist());
    }

    @Override
    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    @Override public Album setId(Long id) {
        this.id.set(id);
        return this;
    }

    @Override public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    @Override public Album setName(String name) {
        this.name.set(name);
        return this;
    }

    @Override public Integer getReleaseYear() {
        return releaseYear.get();
    }

    public IntegerProperty releaseYearProperty() {
        return releaseYear;
    }

    @Override public Album setReleaseYear(Integer year) {
        this.releaseYear.set(year);
        return this;
    }

    @Override public Short getTrackTotal() {
        return (short) trackTotal.get();
    }

    public IntegerProperty trackTotalProperty() {
        return trackTotal;
    }

    public Album setTrackTotal(final Short trackTotal) {
        this.trackTotal.set(trackTotal);
        return this;
    }

    @Override public Short getDiscTotal() {
        return (short) discTotal.get();
    }

    public IntegerProperty discTotalProperty() {
        return discTotal;
    }

    public Album setDiscTotal(final Short discTotal) {
        this.discTotal.set(discTotal);
        return this;
    }

    @Override public Artist getArtist() {
        return artist.get();
    }

    public ObjectProperty<Artist> artistProperty() {
        return artist;
    }

    @Override public Album setArtist(ArtistDto artist) {
        this.getArtist().set(artist);
        return this;
    }

    @Override public Album set(AlbumDto albumDto) {
        if (null != albumDto.getId()) {
            this.setId(albumDto.getId());
        }

        if (null != albumDto.getName()) {
            this.setName(albumDto.getName());
        }

        if (null != albumDto.getReleaseYear()) {
            this.setReleaseYear(albumDto.getReleaseYear());
        }

        if (null != albumDto.getTrackTotal()) {
            this.setTrackTotal(albumDto.getTrackTotal());
        }

        if (null != albumDto.getDiscTotal()) {
            this.setDiscTotal(albumDto.getDiscTotal());
        }

        if (null != albumDto.getArtist()) {
            this.setArtist(albumDto.getArtist());
        }

        return this;
    }

}
