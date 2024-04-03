package fr.xahla.musicx.infrastructure.persistence.entity;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.api.data.ArtistInterface;
import jakarta.persistence.*;

@Entity
@Table(name="albums")
public class AlbumEntity implements AlbumInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="album_id")
    private Long id;

    private String name;
    private Integer releaseYear;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private ArtistEntity artist;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return this.id;
    }

    @Override public AlbumEntity setId(Long id) {
        this.id = id;
        return this;
    }

    @Override public ArtistEntity getArtist() {
        return this.artist;
    }

    @Override public AlbumEntity setArtist(ArtistInterface artistInterface) {
        this.artist = new ArtistEntity().set(artistInterface);
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public AlbumEntity setName(String name) {
        this.name = name;
        return this;
    }

    @Override public Integer getReleaseYear() {
        return this.releaseYear;
    }

    @Override public AlbumEntity setReleaseYear(Integer year) {
        this.releaseYear = year;
        return this;
    }

    public AlbumEntity set(AlbumInterface albumInterface) {
        if (null != albumInterface.getId() && 0 != albumInterface.getId()) {
            this.setId(albumInterface.getId());
        }

        if (null != albumInterface.getName()) {
            this.setName(albumInterface.getName());
        }

        if (null != albumInterface.getReleaseYear()) {
            this.setReleaseYear(albumInterface.getReleaseYear());
        }

        if (null != albumInterface.getArtist()) {
            this.setArtist(albumInterface.getArtist());
        }

        return this;
    }
}
