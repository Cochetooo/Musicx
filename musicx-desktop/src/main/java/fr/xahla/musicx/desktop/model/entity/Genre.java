package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.GenreDto;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.genreRepository;

/**
 * Defines the behaviour of a genre for a JavaFX context.
 * @author Cochetooo
 * @since 0.3.0
 */
public class Genre {

    private final GenreDto dto;

    private final LongProperty id;

    private final StringProperty name;

    private ListProperty<Genre> parents;

    public Genre(final GenreDto genre) {
        this.id = new SimpleLongProperty(genre.getId());

        this.name = new SimpleStringProperty(genre.getName());

        this.dto = genre;
    }

    /**
     * @since 0.3.0
     */
    public GenreDto getDto() {
        return dto;
    }

    // Getters - Setters

    public long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Genre setId(final long id) {
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

    public Genre setName(final String name) {
        this.dto.setName(name);
        this.name.set(name);
        return this;
    }

    public ObservableList<Genre> getParents() {
        if (null == parents) {
            parents = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var parentsDto = genreRepository().getParents(dto);
            parentsDto.forEach(parent -> parents.add(new Genre(parent)));
        }

        return parents.get();
    }

    public ListProperty<Genre> parentsProperty() {
        return parents;
    }

    public Genre setParents(final List<Genre> parents) {
        if (null == this.parents) {
            this.parents = new SimpleListProperty<>(FXCollections.observableList(parents));
        } else {
            this.parents.setAll(parents);
        }

        return this;
    }
}
