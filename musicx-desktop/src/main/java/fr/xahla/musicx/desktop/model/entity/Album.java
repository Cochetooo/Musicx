package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import javafx.beans.property.*;

public class Album implements AlbumInterface {

    private final LongProperty id;
    private final StringProperty name;
    private final IntegerProperty releaseYear;
    private final ObjectProperty<Artist> artist;

    public Album() {
        this.id = new SimpleLongProperty();
        this.name = new SimpleStringProperty();
        this.releaseYear = new SimpleIntegerProperty();
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

    @Override public Artist getArtist() {
        return artist.get();
    }

    public ObjectProperty<Artist> artistProperty() {
        return artist;
    }

    @Override public Album setArtist(ArtistInterface artist) {
        this.getArtist().set(artist);
        return this;
    }

    @Override public Album set(AlbumInterface albumInterface) {
        if (null != albumInterface.getId()) {
            this.setId(albumInterface.getId());
        }

        if (null != albumInterface.getName()) {
            this.setName(albumInterface.getName());
        }

        if (null != albumInterface.getReleaseYear()) {
            this.setReleaseYear(albumInterface.getReleaseYear());
        }

        if (null != albumInterface.getArtist()) {
            this.setArtist(albumInterface.getArtist());
        }

        return this;
    }

}
