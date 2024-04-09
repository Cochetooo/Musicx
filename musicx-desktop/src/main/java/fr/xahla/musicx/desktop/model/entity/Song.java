package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.SongInterface;
import javafx.beans.property.*;

public class Song implements SongInterface {

    private final LongProperty id;
    private final IntegerProperty duration;
    private final IntegerProperty bitRate;
    private final IntegerProperty sampleRate;
    private final StringProperty title;
    private final StringProperty filePath;
    private final BooleanProperty available;
    private final StringProperty format;

    private final ObjectProperty<Album> album;
    private final ObjectProperty<Artist> artist;

    public Song() {
        this.id = new SimpleLongProperty();
        this.duration = new SimpleIntegerProperty();
        this.bitRate = new SimpleIntegerProperty();
        this.sampleRate = new SimpleIntegerProperty();
        this.title = new SimpleStringProperty();
        this.filePath = new SimpleStringProperty();
        this.available = new SimpleBooleanProperty();
        this.format = new SimpleStringProperty();
        this.album = new SimpleObjectProperty<>(new Album());
        this.artist = new SimpleObjectProperty<>(new Artist());
    }

    public Song set(SongInterface song) {
        if (null != song.getId()) {
            this.setId(song.getId());
        }

        if (null != song.getTitle()) {
            this.setTitle(song.getTitle());
        }

        if (null != song.getDuration()) {
            this.setDuration(song.getDuration());
        }

        if (null != song.getBitRate()) {
            this.setBitRate(song.getBitRate());
        }

        if (null != song.getSampleRate()) {
            this.setSampleRate(song.getSampleRate());
        }

        if (null != song.getAlbum()) {
            this.setAlbum(song.getAlbum());
        }

        if (null != song.getArtist()) {
            this.setArtist(song.getArtist());
        }

        if (null != song.getFilePath()) {
            this.setFilePath(song.getFilePath());

            this.setFormat(
                this.getFilePath().substring(this.getFilePath().lastIndexOf('.') + 1).toUpperCase()
            );
        }

        if (null != song.isAvailable()) {
            this.setAvailable(song.isAvailable());
        }

        return this;
    }

    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Song setId(final Long id) {
        this.id.set(id);
        return this;
    }

    @Override
    public Album getAlbum() {
        return this.album.get();
    }

    public ObjectProperty<Album> albumProperty() {
        return album;
    }

    @Override public Song setAlbum(final AlbumInterface album) {
        this.getAlbum().set(album);
        return this;
    }

    public Integer getDuration() {
        return duration.get();
    }

    public IntegerProperty durationProperty() {
        return duration;
    }

    public Song setDuration(final Integer duration) {
        this.duration.set(duration);
        return this;
    }

    public String getTitle() {
        return title.get();
    }

    public StringProperty titleProperty() {
        return title;
    }

    public Song setTitle(final String title) {
        this.title.set(title);
        return this;
    }

    public String getFilePath() {
        return filePath.get();
    }

    public StringProperty filePathProperty() {
        return filePath;
    }

    public Song setFilePath(final String filePath) {
        this.filePath.set(filePath);
        return this;
    }

    @Override public Boolean isAvailable() {
        return available.get();
    }

    public BooleanProperty availableProperty() {
        return available;
    }

    public Song setAvailable(final Boolean available) {
        this.available.set(available);
        return this;
    }

    public String getFormat() {
        return format.get();
    }

    public StringProperty formatProperty() {
        return format;
    }

    public Song setFormat(final String format) {
        this.format.set(format);
        return this;
    }

    @Override
    public Artist getArtist() {
        return artist.get();
    }

    public ObjectProperty<Artist> artistProperty() {
        return artist;
    }

    public Song setArtist(final ArtistInterface artist) {
        this.getArtist().set(artist);
        return this;
    }

    @Override public Integer getBitRate() {
        return bitRate.get();
    }

    public IntegerProperty bitRateProperty() {
        return bitRate;
    }

    @Override public Song setBitRate(final Integer bitRate) {
        this.bitRate.set(bitRate);
        return this;
    }

    @Override public Integer getSampleRate() {
        return sampleRate.get();
    }

    public IntegerProperty sampleRateProperty() {
        return sampleRate;
    }

    @Override public Song setSampleRate(final Integer sampleRate) {
        this.sampleRate.set(sampleRate);
        return this;
    }

}
