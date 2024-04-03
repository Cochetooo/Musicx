package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.api.data.SongInterface;

public class Song implements SongInterface {

    private Long id;
    private ArtistInterface artist;
    private AlbumInterface album;
    private String title;
    private Integer duration;
    private String filePath;
    private Integer bitRate;
    private Integer sampleRate;

    @Override
    public Long getId() {
        return this.id;
    }

    @Override
    public Song setId(Long id) {
        this.id = id;
        return this;
    }

    @Override
    public ArtistInterface getArtist() {
        return this.artist;
    }

    @Override
    public Song setArtist(ArtistInterface artist) {
        this.artist = artist;
        return this;
    }

    @Override
    public AlbumInterface getAlbum() {
        return this.album;
    }

    @Override
    public Song setAlbum(AlbumInterface album) {
        this.album = album;
        return this;
    }

    @Override
    public String getTitle() {
        return this.title;
    }

    @Override
    public Song setTitle(String title) {
        this.title = title;
        return this;
    }

    @Override public Integer getDuration() {
        return this.duration;
    }

    @Override public Song setDuration(Integer duration) {
        this.duration = duration;
        return this;
    }

    @Override public String getFilePath() {
        return this.filePath;
    }

    @Override public Song setFilePath(String filePath) {
        this.filePath = filePath;
        return this;
    }

    @Override public Integer getBitRate() {
        return this.bitRate;
    }

    @Override public Song setBitRate(Integer bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    @Override public Integer getSampleRate() {
        return this.sampleRate;
    }

    @Override public Song setSampleRate(Integer sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

}
