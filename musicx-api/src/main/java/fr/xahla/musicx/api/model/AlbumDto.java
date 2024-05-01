package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.Role;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

/** <b>(API) Interface for Album Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class AlbumDto {

    private Long id;
    private ArtistDto artist;
    private Map<ArtistDto, Role> creditArtists;
    private String name;
    private String catalogNo;
    private LocalDate releaseDate;
    private List<GenreDto> primaryGenres;
    private List<GenreDto> secondaryGenres;
    private short trackTotal;
    private short discTotal;
    private String artworkUrl;
    private LabelDto label;

    public Long getId() {
        return id;
    }

    public AlbumDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public ArtistDto getArtist() {
        return artist;
    }

    public AlbumDto setArtist(final ArtistDto artist) {
        this.artist = artist;
        return this;
    }

    public Map<ArtistDto, Role> getCreditArtists() {
        return creditArtists;
    }

    public AlbumDto setCreditArtists(final Map<ArtistDto, Role> creditArtists) {
        this.creditArtists = creditArtists;
        return this;
    }

    public String getName() {
        return name;
    }

    public AlbumDto setName(final String name) {
        this.name = name;
        return this;
    }

    public String getCatalogNo() {
        return catalogNo;
    }

    public AlbumDto setCatalogNo(final String catalogNo) {
        this.catalogNo = catalogNo;
        return this;
    }

    public LocalDate getReleaseDate() {
        return releaseDate;
    }

    public AlbumDto setReleaseDate(final LocalDate date) {
        this.releaseDate = date;
        return this;
    }

    public List<GenreDto> getPrimaryGenres() {
        return primaryGenres;

    }
    public AlbumDto setPrimaryGenres(final List<GenreDto> genres) {
        this.primaryGenres = genres;
        return this;
    }

    public List<GenreDto> getSecondaryGenres() {
        return secondaryGenres;
    }

    public AlbumDto setSecondaryGenres(final List<GenreDto> genres) {
        this.secondaryGenres = genres;
        return this;
    }

    public short getTrackTotal() {
        return trackTotal;
    }

    public AlbumDto setTrackTotal(final Short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    public short getDiscTotal() {
        return discTotal;
    }

    public AlbumDto setDiscTotal(final Short discTotal) {
        this.discTotal = discTotal;
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl;
    }

    public AlbumDto setArtworkUrl(final String artworkUrl) {
        this.artworkUrl = artworkUrl;
        return this;
    }

    public LabelDto getLabel() {
        return label;
    }

    public AlbumDto setLabel(final LabelDto label) {
        this.label = label;
        return this;
    }

}
