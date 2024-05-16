package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.LabelDto;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.labelRepository;

/**
 * Defines the behaviour of a label for a JavaFX context.
 * @author Cochetooo
 * @since 0.3.0
 */
public class Label {

    private final LabelDto dto;

    private final LongProperty id;

    private final StringProperty name;

    private ListProperty<Genre> genres;
    private ListProperty<Album> releases;

    public Label(final LabelDto label) {
        this.id = new SimpleLongProperty(label.getId());

        this.name = new SimpleStringProperty(label.getName());

        this.dto = label;
    }

    /**
     * @since 0.3.0
     */
    public LabelDto getDto() {
        return dto;
    }

    // Getters - Setters

    public long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Label setId(final long id) {
        this.dto.setId(id);
        this.id.set(id);
        return this;
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Label setName(final String name) {
        this.dto.setName(name);
        this.name.set(name);
        return this;
    }

    public ObservableList<Genre> getGenres() {
        if (null == genres) {
            genres = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var genresDto = labelRepository().getGenres(dto);
            genresDto.forEach(genre -> genres.add(new Genre(genre)));
        }

        return genres.get();
    }

    public ListProperty<Genre> genresProperty() {
        return genres;
    }

    public Label setGenres(final List<Genre> genres) {
        if (null == this.genres) {
            this.genres = new SimpleListProperty<>(FXCollections.observableList(genres));
        } else {
            this.genres.setAll(genres);
        }

        return this;
    }

    public ObservableList<Album> getReleases() {
        return releases.get();
    }

    public ListProperty<Album> releasesProperty() {
        return releases;
    }

    public Label setReleases(final ObservableList<Album> releases) {
        this.releases.set(releases);
        return this;
    }
}
