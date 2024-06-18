package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;
import org.hibernate.Interceptor;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/**
 * Song persistence class for database.
 * @author Cochetooo
 * @since 0.3.0
 */
@Entity
@Table(name = "song")
@Getter
@Setter
public class SongEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, updatable = false)
    private LocalDateTime createdAt;

    private LocalDateTime updatedAt;

    @Column(name = "album_id")
    private Long albumId;

    @Column(name = "artist_id")
    private Long artistId;

    @ElementCollection(fetch = FetchType.EAGER)
    @CollectionTable(name = "song_lyrics", joinColumns = @JoinColumn(name = "song_id"))
    @MapKeyColumn(name = "line_number")
    @Column(name = "lyric")
    private Map<Long, String> lyrics;

    private Integer bitRate;
    private Short discNumber;
    private Long duration;
    private Boolean favourite;
    private String filepath;
    private AudioFormat format;
    private Byte rating;
    private Integer sampleRate;
    private String title;
    private Short trackNumber;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
        name = "song_primary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres = new ArrayList<>();

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(name = "song_secondary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres = new ArrayList<>();

    // Casts

    /**
     * @since 0.3.0
     */
    public SongEntity fromDto(final SongDto songDto) {
        this.setId(songDto.getId());
        this.setAlbumId(songDto.getAlbumId());
        this.setArtistId(songDto.getArtistId());
        this.setBitRate(songDto.getBitRate());
        this.setCreatedAt(songDto.getCreatedAt());
        this.setDiscNumber(songDto.getDiscNumber());
        this.setDuration(songDto.getDuration());
        this.setFavourite(songDto.getFavourite());
        this.setFilepath(songDto.getFilepath());
        this.setFormat(songDto.getFormat());
        this.setLyrics(songDto.getLyrics());
        this.setRating(songDto.getRating());
        this.setSampleRate(songDto.getSampleRate());
        this.setTitle(songDto.getTitle());
        this.setTrackNumber(songDto.getTrackNumber());

        if (null != songDto.getPrimaryGenreIds()) {
            primaryGenres = new ArrayList<>();
            Hibernate.initialize(primaryGenres);
            primaryGenres.clear();
            songDto.getPrimaryGenreIds().forEach(genreId -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.getPrimaryGenres().add(genre);
            });
        }

        if (null != songDto.getSecondaryGenreIds()) {
            secondaryGenres = new ArrayList<>();
            Hibernate.initialize(secondaryGenres);
            secondaryGenres.clear();
            songDto.getSecondaryGenreIds().forEach(genreId -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.getSecondaryGenres().add(genre);
            });
        }

        return this;
    }

    /**
     * @since 0.3.0
     */
    public SongDto toDto() {
        final var songDto = SongDto.builder()
            .id(id)
            .createdAt(createdAt)
            .albumId(albumId)
            .artistId(artistId)
            .bitRate(bitRate)
            .discNumber(discNumber)
            .duration(duration)
            .favourite(favourite)
            .filepath(filepath)
            .format(format)
            .lyrics(lyrics)
            .rating(rating)
            .sampleRate(sampleRate)
            .title(title)
            .trackNumber(trackNumber)
            .build();

        if (null != primaryGenres && Hibernate.isInitialized(primaryGenres)) {
            songDto.setPrimaryGenreIds(primaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        if (null != secondaryGenres && Hibernate.isInitialized(secondaryGenres)) {
            songDto.setSecondaryGenreIds(secondaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        return songDto;
    }

    /**
     * @since 0.3.3
     */
    @PrePersist protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    /**
     * @since 0.4.1
     */
    @PreUpdate protected void onUpdate() {
        this.updatedAt = LocalDateTime.now();
    }
}
