package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;
import jakarta.persistence.criteria.CriteriaBuilder;
import javafx.beans.property.*;

public class SongViewModel implements ViewModelInterface, SongInterface {

    private final LongProperty id;
    private final IntegerProperty duration;
    private final IntegerProperty bitRate;
    private final IntegerProperty sampleRate;
    private final StringProperty title;
    private final StringProperty filePath;

    private final StringProperty format;

    private final ObjectProperty<AlbumViewModel> album;
    private final ObjectProperty<ArtistViewModel> artist;

    public SongViewModel() {
        this.id = new SimpleLongProperty();
        this.duration = new SimpleIntegerProperty();
        this.bitRate = new SimpleIntegerProperty();
        this.sampleRate = new SimpleIntegerProperty();
        this.title = new SimpleStringProperty();
        this.filePath = new SimpleStringProperty();
        this.format = new SimpleStringProperty();
        this.album = new SimpleObjectProperty<>(new AlbumViewModel());
        this.artist = new SimpleObjectProperty<>(new ArtistViewModel());
    }

    public SongViewModel set(SongInterface song) {
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

        return this;
    }

    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public SongViewModel setId(Long id) {
        this.id.set(id);
        return this;
    }

    @Override
    public AlbumViewModel getAlbum() {
        return this.album.get();
    }

    public ObjectProperty<AlbumViewModel> albumProperty() {
        return album;
    }

    @Override public SongViewModel setAlbum(AlbumInterface album) {
        this.getAlbum().set(album);
        return this;
    }

    public Integer getDuration() {
        return duration.get();
    }

    public IntegerProperty durationProperty() {
        return duration;
    }

    public SongViewModel setDuration(Integer duration) {
        this.duration.set(duration);
        return this;
    }

    public String getTitle() {
        return title.get();
    }

    public StringProperty titleProperty() {
        return title;
    }

    public SongViewModel setTitle(String title) {
        this.title.set(title);
        return this;
    }

    public String getFilePath() {
        return filePath.get();
    }

    public StringProperty filePathProperty() {
        return filePath;
    }

    public SongViewModel setFilePath(String filePath) {
        this.filePath.set(filePath);
        return this;
    }

    public String getFormat() {
        return format.get();
    }

    public StringProperty formatProperty() {
        return format;
    }

    public SongViewModel setFormat(String format) {
        this.format.set(format);
        return this;
    }

    @Override
    public ArtistViewModel getArtist() {
        return artist.get();
    }

    public ObjectProperty<ArtistViewModel> artistProperty() {
        return artist;
    }

    public SongViewModel setArtist(ArtistInterface artist) {
        this.getArtist().set(artist);
        return this;
    }

    @Override public Integer getBitRate() {
        return bitRate.get();
    }

    public IntegerProperty bitRateProperty() {
        return bitRate;
    }

    @Override public SongViewModel setBitRate(Integer bitRate) {
        this.bitRate.set(bitRate);
        return this;
    }

    @Override public Integer getSampleRate() {
        return sampleRate.get();
    }

    public IntegerProperty sampleRateProperty() {
        return sampleRate;
    }

    @Override public SongViewModel setSampleRate(Integer sampleRate) {
        this.sampleRate.set(sampleRate);
        return this;
    }
}
