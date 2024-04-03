package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;
import javafx.beans.property.*;

public class AlbumViewModel implements ViewModelInterface, AlbumInterface {

    private final LongProperty id;
    private final StringProperty name;
    private final IntegerProperty releaseYear;
    private final ObjectProperty<ArtistViewModel> artist;

    public AlbumViewModel() {
        this.id = new SimpleLongProperty();
        this.name = new SimpleStringProperty();
        this.releaseYear = new SimpleIntegerProperty();
        this.artist = new SimpleObjectProperty<>(new ArtistViewModel());
    }

    @Override
    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    @Override public AlbumViewModel setId(Long id) {
        this.id.set(id);
        return this;
    }

    @Override public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    @Override public AlbumViewModel setName(String name) {
        this.name.set(name);
        return this;
    }

    @Override public Integer getReleaseYear() {
        return releaseYear.get();
    }

    public IntegerProperty releaseYearProperty() {
        return releaseYear;
    }

    @Override public AlbumViewModel setReleaseYear(Integer year) {
        this.releaseYear.set(year);
        return this;
    }

    @Override public ArtistViewModel getArtist() {
        return artist.get();
    }

    public ObjectProperty<ArtistViewModel> artistProperty() {
        return artist;
    }

    @Override public AlbumViewModel setArtist(ArtistInterface artist) {
        this.getArtist().set(artist);
        return this;
    }

    public AlbumViewModel set(AlbumInterface albumInterface) {
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
