package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.data.ArtistInterface;

public class Artist implements ArtistInterface {

    private Long id;
    private String name;

    @Override
    public Long getId() {
        return this.id;
    }

    @Override
    public Artist setId(Long id) {
        this.id = id;
        return this;
    }

    @Override
    public String getName() {
        return this.name;
    }

    @Override
    public Artist setName(String name) {
        this.name = name;
        return this;
    }
}
