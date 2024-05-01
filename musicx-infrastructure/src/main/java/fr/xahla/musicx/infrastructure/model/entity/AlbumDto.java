package fr.xahla.musicx.infrastructure.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
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
public class AlbumDto implements fr.xahla.musicx.api.model.AlbumDto {
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
    private fr.xahla.musicx.infrastructure.model.entity.ArtistDto artist;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return this.id;
    }

    @Override public AlbumDto setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public fr.xahla.musicx.infrastructure.model.entity.ArtistDto getArtist() {
        return this.artist;
    }

    @Override public AlbumDto setArtist(final ArtistDto artistDto) {
        this.artist = new fr.xahla.musicx.infrastructure.model.entity.ArtistDto().set(artistDto);
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public AlbumDto setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public Integer getReleaseYear() {
        return this.releaseYear;
    }

    @Override public AlbumDto setReleaseYear(final Integer year) {
        this.releaseYear = year;
        return this;
    }

    @Override public Short getTrackTotal() {
        return trackTotal;
    }

    @Override public AlbumDto setTrackTotal(final Short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    @Override public Short getDiscTotal() {
        return discTotal;
    }

    @Override public AlbumDto setDiscTotal(final Short discTotal) {
        this.discTotal = discTotal;
        return this;
    }

    public AlbumDto set(final fr.xahla.musicx.api.model.AlbumDto albumDto) {
        if (null != albumDto.getId() && 0 != albumDto.getId()) {
            this.setId(albumDto.getId());
        }

        if (null != albumDto.getName()) {
            this.setName(albumDto.getName());
        }

        if (null != albumDto.getReleaseYear()) {
            this.setReleaseYear(albumDto.getReleaseYear());
        }

        if (null != albumDto.getTrackTotal()) {
            this.setTrackTotal(albumDto.getTrackTotal());
        }

        if (null != albumDto.getDiscTotal()) {
            this.setDiscTotal(albumDto.getDiscTotal());
        }

        if (null != albumDto.getArtist()) {
            this.setArtist(albumDto.getArtist());
        }

        return this;
    }
}
