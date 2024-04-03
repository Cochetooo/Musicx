package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;

public class Album implements AlbumInterface {
    private Long id;
    private ArtistInterface artist;
    private String name;
    private Integer releaseYear;

    @Override public Long getId() {
        return this.id;
    }

    @Override public Album setId(Long id) {
        this.id = id;
        return this;
    }

    @Override public ArtistInterface getArtist() {
        return this.artist;
    }

    @Override public Album setArtist(ArtistInterface artistInterface) {
        this.artist = artistInterface;
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public Album setName(String name) {
        this.name = name;
        return this;
    }

    @Override public Integer getReleaseYear() {
        return this.releaseYear;
    }

    @Override public Album setReleaseYear(Integer year) {
        this.releaseYear = year;
        return this;
    }
}
