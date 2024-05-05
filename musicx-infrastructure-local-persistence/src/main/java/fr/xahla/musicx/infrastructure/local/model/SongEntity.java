package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.data.AlbumInterface;
import fr.xahla.musicx.api.model.data.ArtistInterface;
import fr.xahla.musicx.api.model.data.GenreInterface;
import fr.xahla.musicx.api.model.data.SongInterface;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import jakarta.persistence.*;
import org.hibernate.Hibernate;

import java.util.List;
import java.util.Map;

@Entity
@Table(name = "song")
public class SongEntity implements SongInterface {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    private AlbumEntity album;

    @ManyToOne(fetch = FetchType.LAZY)
    private ArtistEntity artist;

    @ElementCollection
    @CollectionTable(name = "song_lyrics", joinColumns = @JoinColumn(name = "song_id"))
    @MapKeyColumn(name = "line_number")
    @Column(name = "lyric")
    private Map<Long, String> lyrics;

    private int bitRate;
    private short discNumber;
    private long duration;
    private AudioFormat format;
    private int sampleRate;
    private String title;
    private short trackNumber;

    @ManyToMany
    @JoinTable(
        name = "song_primary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres;

    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "song_secondary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres;

    // Casts

    @Override public SongEntity fromDto(final SongDto songDto) {
        this.setId(songDto.getId())
            .setTitle(songDto.getTitle())
            .setFormat(songDto.getFormat())
            .setLyrics(songDto.getLyrics())
            .setDuration(songDto.getDuration())
            .setBitRate(songDto.getBitRate())
            .setSampleRate(songDto.getSampleRate())
            .setTrackNumber(songDto.getTrackNumber())
            .setDiscNumber(songDto.getDiscNumber());

        if (null != songDto.getArtistId()) {
            Hibernate.initialize(artist);
            this.getArtist().setId(songDto.getArtistId());
        }

        if (null != songDto.getAlbumId()) {
            Hibernate.initialize(album);
            this.getAlbum().setId(songDto.getAlbumId());
        }

        if (!songDto.getPrimaryGenreIds().isEmpty()) {
            Hibernate.initialize(primaryGenres);
            primaryGenres.clear();
            songDto.getPrimaryGenreIds().forEach(genreId -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.getPrimaryGenres().add(genre);
            });
        }

        if (!songDto.getSecondaryGenreIds().isEmpty()) {
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
        final var songDto = new SongDto()
            .setId(id)
            .setTitle(title)
            .setFormat(format)
            .setLyrics(lyrics)
            .setDuration(duration)
            .setBitRate(bitRate)
            .setSampleRate(sampleRate)
            .setTrackNumber(trackNumber)
            .setDiscNumber(discNumber);

        if (null != artist) {
            songDto.setArtistId(artist.getId());
        }

        if (null != album) {
            songDto.setAlbumId(album.getId());
        }

        if (null != primaryGenres) {
            songDto.setPrimaryGenreIds(primaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        if (null != secondaryGenres) {
            songDto.setSecondaryGenreIds(secondaryGenres.stream()
                .map(GenreEntity::getId)
                .toList());
        }

        return songDto;
    }

    public Long getId() {
        return id;
    }

    public SongEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    public ArtistEntity getArtist() {
        return artist;
    }

    public SongEntity setArtist(final ArtistInterface artist) {
        this.artist = (ArtistEntity) artist;
        return this;
    }

    public AlbumEntity getAlbum() {
        return album;
    }

    public SongEntity setAlbum(final AlbumInterface album) {
        this.album = (AlbumEntity) album;
        return this;
    }

    public String getTitle() {
        return title;
    }

    public SongEntity setTitle(final String title) {
        this.title = title;
        return this;
    }

    public AudioFormat getFormat() {
        return format;
    }

    public SongEntity setFormat(final AudioFormat format) {
        this.format = format;
        return this;
    }

    public Map<Long, String> getLyrics() {
        return lyrics;
    }

    public SongEntity setLyrics(final String lyrics) {
        return this.setLyrics(
            Map.of(0L, lyrics)
        );
    }

    public SongEntity setLyrics(final Map<Long, String> lyrics) {
        this.lyrics = lyrics;
        return this;
    }

    public long getDuration() {
        return duration;
    }

    public SongEntity setDuration(final long duration) {
        this.duration = duration;
        return this;
    }

    public int getBitRate() {
        return bitRate;
    }

    public SongEntity setBitRate(final int bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    public int getSampleRate() {
        return sampleRate;
    }

    public SongEntity setSampleRate(final int sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

    public short getTrackNumber() {
        return trackNumber;
    }

    public SongEntity setTrackNumber(final short trackNumber) {
        this.trackNumber = trackNumber;
        return this;
    }

    public short getDiscNumber() {
        return discNumber;
    }

    public SongEntity setDiscNumber(final short discNumber) {
        this.discNumber = discNumber;
        return this;
    }

    public List<GenreEntity> getPrimaryGenres() {
        return primaryGenres;
    }

    public SongEntity setPrimaryGenres(final List<? extends GenreInterface> primaryGenres) {
        this.primaryGenres = primaryGenres.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }

    public List<GenreEntity> getSecondaryGenres() {
        return secondaryGenres;
    }

    public SongEntity setSecondaryGenres(final List<? extends GenreInterface> secondaryGenres) {
        this.secondaryGenres = secondaryGenres.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }
}
