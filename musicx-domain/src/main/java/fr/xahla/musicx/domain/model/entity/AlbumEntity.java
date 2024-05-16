package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.model.enums.ReleaseType;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * Album persistence class for database
 * @author Cochetooo
 */
@Entity
@Table(name="albums")
@Getter
@Setter
public class AlbumEntity {

    // Primary keys
    @Id
    @GeneratedValue(strategy= GenerationType.IDENTITY)
    private Long id;

    // Relations columns
    @Column(name = "artist_id")
    private Long artistId;

    @Column(name = "label_id")
    private Long labelId;

    @ElementCollection
    @CollectionTable(
        name = "album_credit_artists",
        joinColumns = {@JoinColumn(name="album_id")}
    )
    @MapKeyJoinColumn(name = "artist_id")
    @Enumerated(EnumType.STRING)
    @Column(name = "role")
    private Map<ArtistEntity, ArtistRole> creditArtists = new HashMap<>();

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(name = "album_primary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres = new ArrayList<>();

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(name = "album_secondary_genres",
        joinColumns = @JoinColumn(name = "album_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres = new ArrayList<>();

    // Standard columns
    private String artworkUrl;
    private String catalogNo;
    private short discTotal;
    private String name;
    private LocalDate releaseDate;
    private short trackTotal;
    private ReleaseType type;

    // Casts

    public AlbumEntity fromDto(final AlbumDto albumDto) {
        this.setId(albumDto.getId());
        this.setArtistId(albumDto.getArtistId());
        this.setArtworkUrl(albumDto.getArtworkUrl());
        this.setCatalogNo(albumDto.getCatalogNo());
        this.setDiscTotal(albumDto.getDiscTotal());
        this.setLabelId(albumDto.getLabelId());
        this.setName(albumDto.getName());
        this.setReleaseDate(albumDto.getReleaseDate());
        this.setTrackTotal(albumDto.getTrackTotal());
        this.setType(albumDto.getType());

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

    public AlbumDto toDto() {
        final var albumDto = AlbumDto.builder()
            .id(id)
            .artistId(artistId)
            .artworkUrl(artworkUrl)
            .catalogNo(catalogNo)
            .discTotal(discTotal)
            .labelId(labelId)
            .name(name)
            .releaseDate(releaseDate)
            .trackTotal(trackTotal)
            .type(type)
            .build();

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


}
