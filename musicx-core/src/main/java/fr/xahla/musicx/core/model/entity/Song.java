package fr.xahla.musicx.core.model.entity;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.SongInterface;
import jakarta.persistence.*;

@Entity
@Table(name="songs")
public class Song implements SongInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="song_id")
    private Long id;

    private String title;
    private String filePath;

    private Integer bitRate;
    private Integer duration;
    private Integer sampleRate;

    private Boolean available;

    @ManyToOne
    @JoinColumn(name="album_id")
    private Album album;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private Artist artist;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return id;
    }

    @Override public Song setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public Artist getArtist() {
        return this.artist;
    }

    @Override public Song setArtist(final ArtistInterface artistInterface) {
        this.artist = new Artist().set(artistInterface);
        return this;
    }

    @Override public Album getAlbum() {
        return this.album;
    }

    @Override public Song setAlbum(final AlbumInterface albumInterface) {
        this.album = new Album().set(albumInterface);
        return this;
    }

    @Override public String getTitle() {
        return title;
    }

    @Override public Song setTitle(final String title) {
        this.title = title;
        return this;
    }

    @Override public Integer getDuration() {
        return duration;
    }

    @Override public Song setDuration(final Integer duration) {
        this.duration = duration;
        return this;
    }

    @Override
    public String getFilePath() {
        return this.filePath;
    }

    @Override public Song setFilePath(final String filePath) {
        this.filePath = filePath;
        return this;
    }

    @Override public Integer getBitRate() {
        return this.bitRate;
    }

    @Override public Song setBitRate(final Integer bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    @Override public Integer getSampleRate() {
        return this.sampleRate;
    }

    @Override public Song setSampleRate(final Integer sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

    @Override public Boolean isAvailable() {
        return available;
    }

    @Override public Song setAvailable(Boolean available) {
        this.available = available;
        return this;
    }

    @Override public Song set(final SongInterface songInterface) {
        if (null != songInterface.getId() && 0 != songInterface.getId()) {
            this.setId(songInterface.getId());
        }

        if (null != songInterface.getDuration()) {
            this.setDuration(songInterface.getDuration());
        }

        if (null != songInterface.getBitRate()) {
            this.setBitRate(songInterface.getBitRate());
        }

        if (null != songInterface.getSampleRate()) {
            this.setSampleRate(songInterface.getSampleRate());
        }

        if (null != songInterface.getTitle()) {
            this.setTitle(songInterface.getTitle());
        }

        if (null != songInterface.getFilePath()) {
            this.setFilePath(songInterface.getFilePath());
        }

        if (null != songInterface.isAvailable()) {
            this.setAvailable(songInterface.isAvailable());
        }

        if (null != songInterface.getArtist()) {
            this.setArtist(songInterface.getArtist());
        }

        if (null != songInterface.getAlbum()) {
            this.setAlbum(songInterface.getAlbum());
        }

        return this;
    }

}
