package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

@Entity
@Table(name = "song")
@Getter
@Setter
public class SongEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "album_id")
    private Long albumId;

    @Column(name = "artist_id")
    private Long artistId;

    @ElementCollection(fetch = FetchType.EAGER)
    @CollectionTable(name = "song_lyrics", joinColumns = @JoinColumn(name = "song_id"))
    @MapKeyColumn(name = "line_number")
    @Column(name = "lyric")
    private Map<Long, String> lyrics;

    private int bitRate;
    private short discNumber;
    private long duration;
    private String filepath;
    private AudioFormat format;
    private int sampleRate;
    private String title;
    private short trackNumber;

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

    public SongEntity fromDto(final SongDto songDto) {
        this.setId(songDto.getId());
        this.setAlbumId(songDto.getAlbumId());
        this.setArtistId(songDto.getArtistId());
        this.setBitRate(songDto.getBitRate());
        this.setDiscNumber(songDto.getDiscNumber());
        this.setDuration(songDto.getDuration());
        this.setFilepath(songDto.getFilepath());
        this.setFormat(songDto.getFormat());
        this.setLyrics(songDto.getLyrics());
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

    public SongDto toDto() {
        final var songDto = SongDto.builder()
            .id(id)
            .albumId(albumId)
            .artistId(artistId)
            .bitRate(bitRate)
            .discNumber(discNumber)
            .duration(duration)
            .filepath(filepath)
            .format(format)
            .lyrics(lyrics)
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
}
