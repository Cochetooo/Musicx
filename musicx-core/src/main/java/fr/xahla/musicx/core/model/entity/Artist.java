package fr.xahla.musicx.core.model.entity;

import fr.xahla.musicx.api.model.ArtistInterface;
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
public class Artist implements ArtistInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="artist_id")
    private Long id;

    private String name;

    private String country;

    @Override public Long getId() {
        return this.id;
    }

    @Override public Artist setId(final Long id) {
        this.id = id;
        return this;
    }

    @Override public String getName() {
        return this.name;
    }

    @Override public Artist setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public String getCountry() {
        return country;
    }

    @Override public Artist setCountry(String country) {
        this.country = country;
        return this;
    }

    public Artist set(final ArtistInterface artistInterface) {
        if (null != artistInterface.getId() && 0 != artistInterface.getId()) {
            this.setId(artistInterface.getId());
        }

        if (null != artistInterface.getName()) {
            this.setName(artistInterface.getName());
        }

        if (null != artistInterface.getCountry()) {
            this.setCountry(artistInterface.getCountry());
        }

        return this;
    }
}
