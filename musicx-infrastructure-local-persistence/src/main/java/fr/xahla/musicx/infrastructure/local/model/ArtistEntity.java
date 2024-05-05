package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.data.AlbumInterface;
import fr.xahla.musicx.api.model.data.ArtistInterface;
import fr.xahla.musicx.api.model.data.SongInterface;
import jakarta.persistence.*;

import java.util.List;
import java.util.Locale;

@Entity
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(
    name = "artist_type",
    discriminatorType = DiscriminatorType.STRING
)
@Table(name = "artist")
public class ArtistEntity implements ArtistInterface {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @OneToMany(mappedBy = "artist")
    private List<AlbumEntity> albums;

    private String artworkUrl;

    @Basic(optional = false)
    private Locale country;

    private String name;

    @OneToMany(mappedBy = "artist")
    private List<SongEntity> songs;

    public Long getId() {
        return id;
    }

    public ArtistEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public ArtistEntity fromDto(final ArtistDto artistDto) {
        this.setId(artistDto.getId())
            .setName(artistDto.getName())
            .setCountry(artistDto.getCountry())
            .setArtworkUrl(artistDto.getArtworkUrl());

        return this;
    }

    @Override public ArtistDto toDto() {
        final var artistDto = new ArtistDto();

        artistDto
            .setId(this.getId())
            .setArtworkUrl(this.getArtworkUrl())
            .setCountry(this.getCountry())
            .setName(this.getName());

        return artistDto;
    }

    public String getName() {
        return name;
    }

    public ArtistEntity setName(final String name) {
        this.name = name;
        return this;
    }

    public Locale getCountry() {
        return country;
    }

    public ArtistEntity setCountry(final Locale country) {
        this.country = country;
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl;
    }

    public ArtistEntity setArtworkUrl(final String artworkUrl) {
        this.artworkUrl = artworkUrl;
        return this;
    }

    public List<AlbumEntity> getAlbums() {
        return albums;
    }

    public ArtistEntity setAlbums(final List<? extends AlbumInterface> albums) {
        this.albums = albums.stream()
            .filter(AlbumEntity.class::isInstance)
            .map(AlbumEntity.class::cast)
            .toList();

        return this;
    }

    public List<SongEntity> getSongs() {
        return songs;
    }

    public ArtistEntity setSongs(final List<? extends SongInterface> songs) {
        this.songs = songs.stream()
            .filter(SongEntity.class::isInstance)
            .map(SongEntity.class::cast)
            .toList();

        return this;
    }

}
