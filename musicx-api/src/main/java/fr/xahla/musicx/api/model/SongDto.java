package fr.xahla.musicx.api.model;

import java.util.List;

/** <b>(API) Interface for Song Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class SongDto {

    private Long id;
    private ArtistDto artist;
    private AlbumDto album;
    private String title;
    private String format;
    private String lyrics;
    private long duration;
    private int bitRate;
    private int sampleRate;
    private short trackNumber;
    private short discNumber;
    private List<GenreDto> primaryGenres;
    private List<GenreDto> secondaryGenres;

    public Long getId() {
        return id;
    }

    public SongDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public ArtistDto getArtist() {
        return artist;
    }

    public SongDto setArtist(final ArtistDto artistDto) {
        this.artist = artistDto;
        return this;
    }

    public AlbumDto getAlbum() {
        return album;
    }

    public SongDto setAlbum(final AlbumDto album) {
        this.album = album;
        return this;
    }

    public String getTitle() {
        return title;
    }

    public SongDto setTitle(final String title) {
        this.title = title;
        return this;
    }

    public String getFormat() {
        return format;
    }

    public SongDto setFormat(final String format) {
        this.format = format;
        return this;
    }

    public String getLyrics() {
        return lyrics;
    }

    public SongDto setLyrics(final String lyrics) {
        this.lyrics = lyrics;
        return this;
    }

    public long getDuration() {
        return duration;
    }

    public SongDto setDuration(final long duration) {
        this.duration = duration;
        return this;
    }

    public int getBitRate() {
        return bitRate;
    }

    public SongDto setBitRate(final int bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    public int getSampleRate() {
        return sampleRate;
    }

    public SongDto setSampleRate(final int sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

    public short getTrackNumber() {
        return trackNumber;
    }

    public SongDto setTrackNumber(final short trackNumber) {
        this.trackNumber = trackNumber;
        return this;
    }

    public short getDiscNumber() {
        return discNumber;
    }

    public SongDto setDiscNumber(final short discNumber) {
        this.discNumber = discNumber;
        return this;
    }

    public List<GenreDto> getPrimaryGenres() {
        return primaryGenres;
    }

    public SongDto setPrimaryGenres(final List<GenreDto> primaryGenres) {
        this.primaryGenres = primaryGenres;
        return this;
    }

    public List<GenreDto> getSecondaryGenres() {
        return secondaryGenres;
    }

    public SongDto setSecondaryGenres(final List<GenreDto> secondaryGenres) {
        this.secondaryGenres = secondaryGenres;
        return this;
    }

}
