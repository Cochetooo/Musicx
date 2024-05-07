package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.data.*;
import fr.xahla.musicx.api.model.enums.AlbumType;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import jakarta.persistence.*;
import org.hibernate.Hibernate;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Entity
@Table(name="albums")
public class AlbumEntity implements AlbumInterface {

    // Primary keys
    @Id
    @GeneratedValue(strategy= GenerationType.IDENTITY)
    private Long id;

    // Relations columns
    @ManyToOne(fetch = FetchType.LAZY)
    private ArtistEntity artist;

    @ManyToOne(fetch = FetchType.LAZY)
    private LabelEntity label;

    @ElementCollection
    @CollectionTable(
        name = "album_credit_artists",
        joinColumns = {@JoinColumn(name="album_id")}
    )
    @MapKeyJoinColumn(name = "artist_id")
    @Enumerated(EnumType.STRING)
    @Column(name = "role")
    private Map<ArtistEntity, ArtistRole> creditArtists = new HashMap<>();

    @ManyToMany
    @JoinTable(name = "album_primary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres = new ArrayList<>();

    @ManyToMany
    @JoinTable(name = "album_secondary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres = new ArrayList<>();

    // Standard columns
    private String name;
    private String catalogNo;
    private String artworkUrl;
    private LocalDate releaseDate;
    private short trackTotal;
    private short discTotal;

    // Casts

    @Override public AlbumEntity fromDto(final AlbumDto albumDto) {
        this.setId(albumDto.getId())
            .setName(albumDto.getName())
            .setCatalogNo(albumDto.getCatalogNo())
            .setArtworkUrl(albumDto.getArtworkUrl())
            .setReleaseDate(albumDto.getReleaseDate())
            .setTrackTotal(albumDto.getTrackTotal())
            .setDiscTotal(albumDto.getDiscTotal());

        if (null != albumDto.getArtistId()) {
            artist = new ArtistEntity();
            Hibernate.initialize(artist);
            artist.setId(albumDto.getArtistId());
        }

        if (null != albumDto.getLabelId()) {
            label = new LabelEntity();
            Hibernate.initialize(label);
            label.setId(albumDto.getLabelId());
        }

        if (null != albumDto.getCreditArtistIds()) {
            Hibernate.initialize(creditArtists);
            creditArtists.clear();

            albumDto.getCreditArtistIds().forEach((key, value) -> {
                final var artist = new ArtistEntity();
                artist.setId(key);
                this.getCreditArtists().put(artist, value);
            });
        }

        if (null != albumDto.getPrimaryGenreIds()) {
            Hibernate.initialize(primaryGenres);
            primaryGenres.clear();

            albumDto.getPrimaryGenreIds().forEach((genreId) -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.getPrimaryGenres().add(genre);
            });
        }

        if (null != albumDto.getSecondaryGenreIds()) {
            Hibernate.initialize(secondaryGenres);
            secondaryGenres.clear();

            albumDto.getSecondaryGenreIds().forEach((genreId) -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.getSecondaryGenres().add(genre);
            });
        }

        return this;
    }

    @Override public AlbumDto toDto() {
        final var albumDto = new AlbumDto();

        albumDto
            .setId(id)
            .setName(name)
            .setCatalogNo(catalogNo)
            .setArtworkUrl(artworkUrl)
            .setReleaseDate(releaseDate)
            .setTrackTotal(trackTotal)
            .setDiscTotal(discTotal);

        if (null != this.artist && Hibernate.isInitialized(this.artist)) {
            albumDto.setArtistId(artist.getId());
        }

        if (null != this.label && Hibernate.isInitialized(this.label)) {
            albumDto.setLabelId(label.getId());
        }

        if (null != this.creditArtists && Hibernate.isInitialized(this.creditArtists)) {
            albumDto.setCreditArtistIds(creditArtists.entrySet().stream()
                .collect(Collectors.toMap(
                    entry -> entry.getKey().getId(),
                    Map.Entry::getValue
                )));
        }

        if (null != this.primaryGenres && Hibernate.isInitialized(this.primaryGenres)) {
            albumDto.setPrimaryGenreIds(primaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        if (null != this.secondaryGenres && Hibernate.isInitialized(this.secondaryGenres)) {
            albumDto.setSecondaryGenreIds(secondaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        return albumDto;
    }

    // Getters - Setters

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

    public AlbumEntity setArtist(final ArtistInterface artist) {
        this.artist = (ArtistEntity) artist;
        return this;
    }

    public Map<ArtistEntity, ArtistRole> getCreditArtists() {
        return creditArtists;
    }

    public AlbumEntity setCreditArtists(final Map<? extends ArtistInterface, ArtistRole> creditArtists) {
        this.creditArtists = creditArtists.entrySet().stream()
            .collect(Collectors.toMap(
                entry -> (ArtistEntity) entry.getKey(),
                Map.Entry::getValue
            ));

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

    public AlbumEntity setPrimaryGenres(final List<? extends GenreInterface> primaryGenres) {
        this.primaryGenres = primaryGenres.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }

    public List<GenreEntity> getSecondaryGenres() {
        return secondaryGenres;
    }

    public AlbumEntity setSecondaryGenres(final List<? extends GenreInterface> secondaryGenres) {
        this.secondaryGenres = secondaryGenres.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }

    @Override public List<? extends SongInterface> getSongs() {
        return List.of();
    }

    @Override public AlbumInterface setSongs(final List<? extends SongInterface> songs) {
        return null;
    }

    public short getTrackTotal() {
        return trackTotal;
    }

    public AlbumEntity setTrackTotal(final short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    @Override public AlbumType getType() {
        return null;
    }

    @Override public AlbumInterface setType(final AlbumType type) {
        return null;
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

    public AlbumEntity setLabel(final LabelInterface label) {
        this.label = (LabelEntity) label;
        return this;
    }


}
