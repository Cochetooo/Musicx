package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.enums.Role;
import jakarta.persistence.*;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

@Entity
@Table(name="albums")
public class AlbumEntity {

    @Id
    @GeneratedValue(strategy= GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private ArtistEntity artist;

    @ElementCollection
    @CollectionTable(
        name = "album_credit_artists",
        joinColumns = {@JoinColumn(name="album_id")}
    )
    @MapKeyJoinColumn(name = "artist_id")
    @Column(name = "role")
    private Map<ArtistEntity, Role> creditArtists;

    private String name;
    private String catalogNo;
    private LocalDate releaseDate;

    @ManyToMany
    @JoinTable(name = "album_primary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres;

    @ManyToMany
    @JoinTable(name = "album_secondary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres;

    private short trackTotal;
    private short discTotal;
    private String artworkUrl;

    @ManyToOne
    @JoinColumn(name = "label_name")
    private LabelEntity label;

    public AlbumDto toDto() {
        return new AlbumDto()
            .setId(id)
            .setArtist(artist.toDto())
            .setCreditArtists(null)
            .setName(name)
            .setCatalogNo(catalogNo)
            .setReleaseDate(releaseDate)
            .setPrimaryGenres(primaryGenres.stream().map(GenreEntity::toDto).toList())
            .setSecondaryGenres(secondaryGenres.stream().map(GenreEntity::toDto).toList())
            .setTrackTotal(trackTotal)
            .setDiscTotal(discTotal)
            .setArtworkUrl(artworkUrl)
            .setLabel(label.toDto());
    }

    public Long getId() {
        return id;
    }

    public AlbumEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    public ArtistEntity getArtist() {
        return artist;
    }

    public AlbumEntity setArtist(final ArtistEntity artist) {
        this.artist = artist;
        return this;
    }

    public Map<ArtistEntity, Role> getCreditArtists() {
        return creditArtists;
    }

    public AlbumEntity setCreditArtists(final Map<ArtistEntity, Role> creditArtists) {
        this.creditArtists = creditArtists;
        return this;
    }

    public String getName() {
        return name;
    }

    public AlbumEntity setName(final String name) {
        this.name = name;
        return this;
    }

    public String getCatalogNo() {
        return catalogNo;
    }

    public AlbumEntity setCatalogNo(final String catalogNo) {
        this.catalogNo = catalogNo;
        return this;
    }

    public LocalDate getReleaseDate() {
        return releaseDate;
    }

    public AlbumEntity setReleaseDate(final LocalDate releaseDate) {
        this.releaseDate = releaseDate;
        return this;
    }

    public List<GenreEntity> getPrimaryGenres() {
        return primaryGenres;
    }

    public AlbumEntity setPrimaryGenres(final List<GenreEntity> primaryGenres) {
        this.primaryGenres = primaryGenres;
        return this;
    }

    public List<GenreEntity> getSecondaryGenres() {
        return secondaryGenres;
    }

    public AlbumEntity setSecondaryGenres(final List<GenreEntity> secondaryGenres) {
        this.secondaryGenres = secondaryGenres;
        return this;
    }

    public short getTrackTotal() {
        return trackTotal;
    }

    public AlbumEntity setTrackTotal(final short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    public short getDiscTotal() {
        return discTotal;
    }

    public AlbumEntity setDiscTotal(final short discTotal) {
        this.discTotal = discTotal;
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl;
    }

    public AlbumEntity setArtworkUrl(final String artworkUrl) {
        this.artworkUrl = artworkUrl;
        return this;
    }

    public LabelEntity getLabel() {
        return label;
    }

    public AlbumEntity setLabel(final LabelEntity label) {
        this.label = label;
        return this;
    }
}
