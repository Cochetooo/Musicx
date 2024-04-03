package fr.xahla.musicx.infrastructure.persistence.entity;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.domain.model.Song;
import jakarta.persistence.*;

import java.util.Set;

@Entity
@Table(name="songs")
public class SongEntity implements SongInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="song_id")
    private Long id;

    private String title;
    private String filePath;

    private Integer bitRate;
    private Integer duration;
    private Integer sampleRate;

    @ManyToOne
    @JoinColumn(name="album_id")
    private AlbumEntity album;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private ArtistEntity artist;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return id;
    }

    @Override public SongEntity setId(Long id) {
        this.id = id;
        return this;
    }

    @Override public ArtistEntity getArtist() {
        return this.artist;
    }

    @Override public SongEntity setArtist(ArtistInterface artistInterface) {
        this.artist = new ArtistEntity().set(artistInterface);
        return this;
    }

    @Override public AlbumEntity getAlbum() {
        return this.album;
    }

    @Override public SongEntity setAlbum(AlbumInterface albumInterface) {
        this.album = new AlbumEntity().set(albumInterface);
        return this;
    }

    @Override public String getTitle() {
        return title;
    }

    @Override public SongEntity setTitle(String title) {
        this.title = title;
        return this;
    }

    @Override public Integer getDuration() {
        return duration;
    }

    @Override public SongEntity setDuration(Integer duration) {
        this.duration = duration;
        return this;
    }

    @Override
    public String getFilePath() {
        return this.filePath;
    }

    @Override public SongEntity setFilePath(String filePath) {
        this.filePath = filePath;
        return this;
    }

    @Override public Integer getBitRate() {
        return this.bitRate;
    }

    @Override public SongEntity setBitRate(Integer bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    @Override public Integer getSampleRate() {
        return this.sampleRate;
    }

    @Override public SongInterface setSampleRate(Integer sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }


    public SongEntity set(SongInterface songInterface) {
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

        if (null != songInterface.getArtist()) {
            this.setArtist(songInterface.getArtist());
        }

        if (null != songInterface.getAlbum()) {
            this.setAlbum(songInterface.getAlbum());
        }

        return this;
    }

}
