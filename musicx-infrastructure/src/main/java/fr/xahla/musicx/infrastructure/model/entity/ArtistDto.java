package fr.xahla.musicx.infrastructure.model.entity;

import jakarta.persistence.*;

/** <b>Class that defines the Artist Model.</b>
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
@Table(name="artists")
public class ArtistDto implements fr.xahla.musicx.api.model.ArtistDto {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="artist_id")
    private Long id;

    private String name;

    private String country;

    @Override public Long getId() {
        return this.id;
    }

    @Override public ArtistDto setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public ArtistDto setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public String getCountry() {
        return country;
    }

    @Override public ArtistDto setCountry(String country) {
        this.country = country;
        return this;
    }

    public ArtistDto set(final fr.xahla.musicx.api.model.ArtistDto artistDto) {
        if (null != artistDto.getId() && 0 != artistDto.getId()) {
            this.setId(artistDto.getId());
        }

        if (null != artistDto.getName()) {
            this.setName(artistDto.getName());
        }

        if (null != artistDto.getCountry()) {
            this.setCountry(artistDto.getCountry());
        }

        return this;
    }
}
