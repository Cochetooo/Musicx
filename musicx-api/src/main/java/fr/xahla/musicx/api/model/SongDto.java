package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.AudioFormat;

import java.util.List;
import java.util.Map;

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

    private Long albumId;
    private Long artistId;
    private List<Long> primaryGenreIds;
    private List<Long> secondaryGenreIds;

    private int bitRate;
    private short discNumber;
    private long duration;
    private AudioFormat format;
    private Map<Long, String> lyrics;
    private int sampleRate;
    private String title;
    private short trackNumber;

    public Long getId() {
        return id;
    }

    public SongDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public Long getAlbumId() {
        return albumId;
    }

    public SongDto setAlbumId(final Long albumId) {
        this.albumId = albumId;
        return this;
    }

    public Long getArtistId() {
        return artistId;
    }

    public SongDto setArtistId(final Long artistId) {
        this.artistId = artistId;
        return this;
    }

    public List<Long> getPrimaryGenreIds() {
        return primaryGenreIds;
    }

    public SongDto setPrimaryGenreIds(final List<Long> primaryGenreIds) {
        this.primaryGenreIds = primaryGenreIds;
        return this;
    }

    public List<Long> getSecondaryGenreIds() {
        return secondaryGenreIds;
    }

    public SongDto setSecondaryGenreIds(final List<Long> secondaryGenreIds) {
        this.secondaryGenreIds = secondaryGenreIds;
        return this;
    }

    public int getBitRate() {
        return bitRate;
    }

    public SongDto setBitRate(final int bitRate) {
        this.bitRate = bitRate;
        return this;
    }

    public short getDiscNumber() {
        return discNumber;
    }

    public SongDto setDiscNumber(final short discNumber) {
        this.discNumber = discNumber;
        return this;
    }

    public long getDuration() {
        return duration;
    }

    public SongDto setDuration(final long duration) {
        this.duration = duration;
        return this;
    }

    public AudioFormat getFormat() {
        return format;
    }

    public SongDto setFormat(final AudioFormat format) {
        this.format = format;
        return this;
    }

    public Map<Long, String> getLyrics() {
        return lyrics;
    }

    public SongDto setLyrics(final String lyrics) {
        return this.setLyrics(Map.of(0L, lyrics));
    }

    public SongDto setLyrics(final Map<Long, String> lyrics) {
        this.lyrics = lyrics;
        return this;
    }

    public int getSampleRate() {
        return sampleRate;
    }

    public SongDto setSampleRate(final int sampleRate) {
        this.sampleRate = sampleRate;
        return this;
    }

    public String getTitle() {
        return title;
    }

    public SongDto setTitle(final String title) {
        this.title = title;
        return this;
    }

    public short getTrackNumber() {
        return trackNumber;
    }

    public SongDto setTrackNumber(final short trackNumber) {
        this.trackNumber = trackNumber;
        return this;
    }
}
