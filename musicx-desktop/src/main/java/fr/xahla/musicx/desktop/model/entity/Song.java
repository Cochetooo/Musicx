package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.ObservableMap;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

/** <b>Class that defines the Song Model for desktop view usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Song {

    private final SongDto dto;

    private final LongProperty id;

    private final IntegerProperty bitRate;
    private final IntegerProperty discNumber;
    private final LongProperty duration;
    private final StringProperty filepath;
    private final ObjectProperty<AudioFormat> format;
    private final IntegerProperty sampleRate;
    private final StringProperty title;
    private final IntegerProperty trackNumber;

    private ObjectProperty<Album> album;
    private ObjectProperty<Artist> artist;
    private MapProperty<Long, String> lyrics;
    private ListProperty<Genre> primaryGenres;
    private ListProperty<Genre> secondaryGenres;

    public Song(final SongDto song) {
        this.id = new SimpleLongProperty(song.getId());

        this.bitRate = new SimpleIntegerProperty(song.getBitRate());
        this.discNumber = new SimpleIntegerProperty(song.getDiscNumber());
        this.duration = new SimpleLongProperty(song.getDuration());
        this.filepath = new SimpleStringProperty(song.getFilepath());
        this.format = new SimpleObjectProperty<>(song.getFormat());
        this.sampleRate = new SimpleIntegerProperty(song.getSampleRate());
        this.title = new SimpleStringProperty(song.getTitle());
        this.trackNumber = new SimpleIntegerProperty(song.getTrackNumber());

        this.lyrics = new SimpleMapProperty<>(FXCollections.observableMap(new HashMap<>()));

        this.dto = song;
    }

    public SongDto toDto() {
        dto.setBitRate(getBitRate());
        dto.setDiscNumber((short) getDiscNumber());
        dto.setDuration(getDuration());
        dto.setFilepath(getFilepath());
        dto.setFormat(getFormat());
        dto.setSampleRate(getSampleRate());
        dto.setTitle(getTitle());
        dto.setTrackNumber((short) getTrackNumber());

        return dto;
    }

    // Getters - Setters

    public long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Song setId(final long id) {
        this.id.set(id);
        return this;
    }

    public int getBitRate() {
        return bitRate.get();
    }

    public IntegerProperty bitRateProperty() {
        return bitRate;
    }

    public Song setBitRate(final int bitRate) {
        this.bitRate.set(bitRate);
        return this;
    }

    public int getDiscNumber() {
        return discNumber.get();
    }

    public IntegerProperty discNumberProperty() {
        return discNumber;
    }

    public Song setDiscNumber(final int discNumber) {
        this.discNumber.set(discNumber);
        return this;
    }

    public long getDuration() {
        return duration.get();
    }

    public LongProperty durationProperty() {
        return duration;
    }

    public Song setDuration(final long duration) {
        this.duration.set(duration);
        return this;
    }

    public String getFilepath() {
        return filepath.get();
    }

    public StringProperty filepathProperty() {
        return filepath;
    }

    public Song setFilepath(final String filepath) {
        this.filepath.set(filepath);
        return this;
    }

    public AudioFormat getFormat() {
        return format.get();
    }

    public ObjectProperty<AudioFormat> formatProperty() {
        return format;
    }

    public Song setFormat(final AudioFormat format) {
        this.format.set(format);
        return this;
    }

    public int getSampleRate() {
        return sampleRate.get();
    }

    public IntegerProperty sampleRateProperty() {
        return sampleRate;
    }

    public Song setSampleRate(final int sampleRate) {
        this.sampleRate.set(sampleRate);
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

    public int getTrackNumber() {
        return trackNumber.get();
    }

    public IntegerProperty trackNumberProperty() {
        return trackNumber;
    }

    public Song setTrackNumber(final int trackNumber) {
        this.trackNumber.set(trackNumber);
        return this;
    }

    public Album getAlbum() {
        if (null == album) {
            album = new SimpleObjectProperty<>();

            final var albumRepos = songRepository().getAlbum(dto);

            if (null != albumRepos) {
                album.set(new Album(albumRepos));
            }
        }

        return album.get();
    }

    public ObjectProperty<Album> albumProperty() {
        return album;
    }

    public Song setAlbum(final Album album) {
        if (null == this.album) {
            this.album = new SimpleObjectProperty<>(album);
        } else {
            this.album.set(album);
        }

        return this;
    }

    public Artist getArtist() {
        if (null == artist) {
            artist = new SimpleObjectProperty<>();

            final var artistRepos = songRepository().getArtist(dto);

            if (null != artistRepos) {
                artist.set(new Artist(artistRepos));
            }
        }

        return artist.get();
    }

    public ObjectProperty<Artist> artistProperty() {
        return artist;
    }

    public Song setArtist(final Artist artist) {
        if (null == this.artist) {
            this.artist = new SimpleObjectProperty<>(artist);
        } else {
            this.artist.set(artist);
        }

        return this;
    }

    public ObservableMap<Long, String> getLyrics() {
        return lyrics.get();
    }

    public MapProperty<Long, String> lyricsProperty() {
        return lyrics;
    }

    public Song setLyrics(final ObservableMap<Long, String> lyrics) {
        this.lyrics.set(lyrics);
        return this;
    }

    public ObservableList<Genre> getPrimaryGenres() {
        if (null == primaryGenres) {
            primaryGenres = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var primaryGenresDto = songRepository().getPrimaryGenres(dto);
            primaryGenresDto.forEach(genre -> primaryGenres.add(new Genre(genre)));
        }

        return primaryGenres.get();
    }

    public ListProperty<Genre> primaryGenresProperty() {
        return primaryGenres;
    }

    public Song setPrimaryGenres(final List<Genre> primaryGenres) {
        if (null == this.primaryGenres) {
            this.primaryGenres = new SimpleListProperty<>(FXCollections.observableList(primaryGenres));
        } else {
            this.primaryGenres.setAll(primaryGenres);
        }

        return this;
    }

    public ObservableList<Genre> getSecondaryGenres() {
        if (null == secondaryGenres) {
            secondaryGenres = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var secondaryGenresDto = songRepository().getSecondaryGenres(dto);
            secondaryGenresDto.forEach(genre -> secondaryGenres.add(new Genre(genre)));
        }

        return secondaryGenres.get();
    }

    public ListProperty<Genre> secondaryGenresProperty() {
        return secondaryGenres;
    }

    public Song setSecondaryGenres(final List<Genre> secondaryGenres) {
        if (null == this.secondaryGenres) {
            this.secondaryGenres = new SimpleListProperty<>(FXCollections.observableList(secondaryGenres));
        } else {
            this.secondaryGenres.setAll(secondaryGenres);
        }

        return this;
    }
}
