package fr.xahla.musicx.infrastructure.external.model;

import fr.xahla.musicx.api.model.GenreInterface;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

public class Genre implements GenreInterface {

    private String name;
    private List<Genre> parents;

    @Override public Genre set(final GenreInterface genreInterface) {
        if (null != genreInterface.getName()) {
            this.setName(genreInterface.getName());
        }

        if (null != genreInterface.getParents()) {
            this.setParents(genreInterface.getParents());
        }

        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public Genre setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public List<Genre> getParents() {
        return this.parents;
    }

    @Override public Genre setParents(final List<? extends GenreInterface> genres) {
        this.parents = genres.stream().map(this::set).toList();
        return this;
    }

}
