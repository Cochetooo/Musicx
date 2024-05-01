package fr.xahla.musicx.infrastructure.model.entity;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import jakarta.persistence.*;

import java.util.List;

/** <b>Class that defines the Song Model.</b>
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
@Table(name="songs")
public class SongDto implements fr.xahla.musicx.api.model.SongDto {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="song_id")
    private Long id;

    private String title;
    private String filePath;

    private Integer bitRate;
    private Integer duration;
    private Integer sampleRate;

    private Short trackNumber;
    private Short discNumber;

    private Boolean available;

    @ManyToOne
    @JoinColumn(name="album_id")
    private fr.xahla.musicx.infrastructure.model.entity.AlbumDto album;

    @ManyToOne
    @JoinColumn(name="artist_id")
    private fr.xahla.musicx.infrastructure.model.entity.ArtistDto artist;

    @ElementCollection(fetch=FetchType.EAGER)
    private List<String> primaryGenres;

    @ElementCollection(fetch=FetchType.EAGER)
    private List<String> secondaryGenres;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    @Override public Long getId() {
        return id;
    }

    @Override public SongDto setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public fr.xahla.musicx.infrastructure.model.entity.ArtistDto getArtist() {
        return this.artist;
    }

    @Override public SongDto setArtist(final ArtistDto artistDto) {
        this.artist = new fr.xahla.musicx.infrastructure.model.entity.ArtistDto().set(artistDto);
        return this;
    }

    @Override public fr.xahla.musicx.infrastructure.model.entity.AlbumDto getAlbum() {
        return this.album;
    }

    @Override public SongDto setAlbum(final AlbumDto albumDto) {
        this.album = new fr.xahla.musicx.infrastructure.model.entity.AlbumDto().set(albumDto);
        return this;
    }

    @Override public String getTitle() {
        return title;
    }

    @Override public SongDto setTitle(final String title) {
        this.title = title;
        return this;
    }

    @Override public Integer getDuration() {
        return duration;
    }

    @Override public SongDto setDuration(final Integer duration) {
        this.duration = duration;
        return this;
    }

    @Override
    public String getFilePath() {
        return this.filePath;
    }

    @Override public SongDto setFilePath(final String filePath) {
        this.filePath = filePath;
        return this;
    }

    @Override public Integer getBitRate() {
        return this.bitRate;
    }

    @Override public SongDto setBitRate(final Integer bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    @Override public Integer getSampleRate() {
        return this.sampleRate;
    }

    @Override public SongDto setSampleRate(final Integer sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

    @Override public Short getTrackNumber() {
        return trackNumber;
    }

    @Override public SongDto setTrackNumber(final Short trackNumber) {
        this.trackNumber = trackNumber;
        return this;
    }

    @Override public Short getDiscNumber() {
        return discNumber;
    }

    @Override public SongDto setDiscNumber(final Short discNumber) {
        this.discNumber = discNumber;
        return this;
    }

    @Override public Boolean isAvailable() {
        return available;
    }

    @Override public SongDto setAvailable(Boolean available) {
        this.available = available;
        return this;
    }

    @Override
    public List<String> getPrimaryGenres() {
        return primaryGenres;
    }

    @Override
    public SongDto setPrimaryGenres(final List<String> primaryGenres) {
        this.primaryGenres = primaryGenres;
        return this;
    }

    @Override
    public List<String> getSecondaryGenres() {
        return secondaryGenres;
    }

    @Override
    public SongDto setSecondaryGenres(final List<String> secondaryGenres) {
        this.secondaryGenres = secondaryGenres;
        return this;
    }

    @Override public SongDto set(final fr.xahla.musicx.api.model.SongDto songDto) {
        if (null != songDto.getId() && 0 != songDto.getId()) {
            this.setId(songDto.getId());
        }

        if (null != songDto.getDuration()) {
            this.setDuration(songDto.getDuration());
        }

        if (null != songDto.getBitRate()) {
            this.setBitRate(songDto.getBitRate());
        }

        if (null != songDto.getSampleRate()) {
            this.setSampleRate(songDto.getSampleRate());
        }

        if (null != songDto.getTrackNumber()) {
            this.setTrackNumber(songDto.getTrackNumber());
        }

        if (null != songDto.getDiscNumber()) {
            this.setDiscNumber(songDto.getDiscNumber());
        }

        if (null != songDto.getTitle()) {
            this.setTitle(songDto.getTitle());
        }

        if (null != songDto.getFilePath()) {
            this.setFilePath(songDto.getFilePath());
        }

        if (null != songDto.isAvailable()) {
            this.setAvailable(songDto.isAvailable());
        }

        if (null != songDto.getArtist()) {
            this.setArtist(songDto.getArtist());
        }

        if (null != songDto.getAlbum()) {
            this.setAlbum(songDto.getAlbum());
        }

        if (null != songDto.getPrimaryGenres()) {
            this.setPrimaryGenres(songDto.getPrimaryGenres());
        }

        if (null != songDto.getSecondaryGenres()) {
            this.setSecondaryGenres(songDto.getSecondaryGenres());
        }

        return this;
    }

}
