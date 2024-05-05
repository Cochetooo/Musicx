package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.AlbumType;
import fr.xahla.musicx.api.model.enums.ArtistRole;

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

    private Long artistId;
    private Map<Long, ArtistRole> creditArtistIds;
    private Long labelId;
    private List<Long> primaryGenreIds;
    private List<Long> secondaryGenreIds;

    private String artworkUrl;
    private String catalogNo;
    private short discTotal;
    private String name;
    private LocalDate releaseDate;
    private short trackTotal;
    private AlbumType type;

    public Long getId() {
        return id;
    }

    public AlbumDto setId(final Long id) {
        this.id = id;
        return this;
    }

    public Long getArtistId() {
        return artistId;
    }

    public AlbumDto setArtistId(final Long artistId) {
        this.artistId = artistId;
        return this;
    }

    public Long getLabelId() {
        return labelId;
    }

    public AlbumDto setLabelId(final Long labelId) {
        this.labelId = labelId;
        return this;
    }

    public Map<Long, ArtistRole> getCreditArtistIds() {
        return creditArtistIds;
    }

    public AlbumDto setCreditArtistIds(final Map<Long, ArtistRole> creditArtistIds) {
        this.creditArtistIds = creditArtistIds;
        return this;
    }

    public List<Long> getPrimaryGenreIds() {
        return primaryGenreIds;
    }

    public AlbumDto setPrimaryGenreIds(final List<Long> primaryGenreIds) {
        this.primaryGenreIds = primaryGenreIds;
        return this;
    }

    public List<Long> getSecondaryGenreIds() {
        return secondaryGenreIds;
    }

    public AlbumDto setSecondaryGenreIds(final List<Long> secondaryGenreIds) {
        this.secondaryGenreIds = secondaryGenreIds;
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

    public AlbumDto setReleaseDate(final LocalDate releaseDate) {
        this.releaseDate = releaseDate;
        return this;
    }

    public short getTrackTotal() {
        return trackTotal;
    }

    public AlbumDto setTrackTotal(final short trackTotal) {
        this.trackTotal = trackTotal;
        return this;
    }

    public short getDiscTotal() {
        return discTotal;
    }

    public AlbumDto setDiscTotal(final short discTotal) {
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

    public AlbumType getType() {
        return type;
    }

    public AlbumDto setType(final AlbumType type) {
        this.type = type;
        return this;
    }
}
