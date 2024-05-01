package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.SongDto;
import jakarta.persistence.*;

import java.util.List;

@Entity
@Table(name = "song")
public class SongEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "artist_id")
    private ArtistEntity artist;

    @ManyToOne
    @JoinColumn(name = "album_id")
    private AlbumEntity album;

    private String title;
    private String format;
    private String lyrics;
    private long duration;
    private int bitRate;
    private int sampleRate;
    private short trackNumber;
    private short discNumber;

    @ManyToMany
    @JoinTable(name = "song_primary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> primaryGenres;

    @ManyToMany
    @JoinTable(name = "song_secondary_genres",
        joinColumns = @JoinColumn(name = "song_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> secondaryGenres;

    public SongDto toDto() {
        return new SongDto()
            .setId(id)
            .setArtist(artist.toDto())
            .setAlbum(album.toDto())
            .setTitle(title)
            .setFormat(format)
            .setLyrics(lyrics)
            .setDuration(duration)
            .setBitRate(bitRate)
            .setSampleRate(sampleRate)
            .setTrackNumber(trackNumber)
            .setDiscNumber(discNumber);
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

    public SongEntity setArtist(final ArtistEntity artist) {
        this.artist = artist;
        return this;
    }

    public AlbumEntity getAlbum() {
        return album;
    }

    public SongEntity setAlbum(final AlbumEntity album) {
        this.album = album;
        return this;
    }

    public String getTitle() {
        return title;
    }

    public SongEntity setTitle(final String title) {
        this.title = title;
        return this;
    }

    public String getFormat() {
        return format;
    }

    public SongEntity setFormat(final String format) {
        this.format = format;
        return this;
    }

    public String getLyrics() {
        return lyrics;
    }

    public SongEntity setLyrics(final String lyrics) {
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

    public SongEntity setPrimaryGenres(final List<GenreEntity> primaryGenres) {
        this.primaryGenres = primaryGenres;
        return this;
    }

    public List<GenreEntity> getSecondaryGenres() {
        return secondaryGenres;
    }

    public SongEntity setSecondaryGenres(final List<GenreEntity> secondaryGenres) {
        this.secondaryGenres = secondaryGenres;
        return this;
    }
}
