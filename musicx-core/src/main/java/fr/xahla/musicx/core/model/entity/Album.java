package fr.xahla.musicx.core.model.entity;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import jakarta.persistence.*;

/** <b>Class that defines the Album Model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
@Entity
@Table(name="albums")
public class Album implements AlbumInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="album_id")
    private Long id;

    private String name;
    private Integer releaseYear;

    private Short trackTotal;
    private Short discTotal;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private Artist artist;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return this.id;
    }

    @Override public Album setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public Artist getArtist() {
        return this.artist;
    }

    @Override public Album setArtist(final ArtistInterface artistInterface) {
        this.artist = new Artist().set(artistInterface);
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public Album setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public Integer getReleaseYear() {
        return this.releaseYear;
    }

    @Override public Album setReleaseYear(final Integer year) {
        this.releaseYear = year;
        return this;
    }

    @Override public Short getTrackTotal() {
        return trackTotal;
    }

    @Override public Album setTrackTotal(final Short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    @Override public Short getDiscTotal() {
        return discTotal;
    }

    @Override public Album setDiscTotal(final Short discTotal) {
        this.discTotal = discTotal;
        return this;
    }

    public Album set(final AlbumInterface albumInterface) {
        if (null != albumInterface.getId() && 0 != albumInterface.getId()) {
            this.setId(albumInterface.getId());
        }

        if (null != albumInterface.getName()) {
            this.setName(albumInterface.getName());
        }

        if (null != albumInterface.getReleaseYear()) {
            this.setReleaseYear(albumInterface.getReleaseYear());
        }

        if (null != albumInterface.getTrackTotal()) {
            this.setTrackTotal(albumInterface.getTrackTotal());
        }

        if (null != albumInterface.getDiscTotal()) {
            this.setDiscTotal(albumInterface.getDiscTotal());
        }

        if (null != albumInterface.getArtist()) {
            this.setArtist(albumInterface.getArtist());
        }

        return this;
    }
}
