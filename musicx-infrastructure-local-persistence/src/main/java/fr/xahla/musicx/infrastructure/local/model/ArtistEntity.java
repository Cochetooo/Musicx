package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.BandArtistDto;
import fr.xahla.musicx.api.model.PersonArtistDto;
import jakarta.persistence.*;

import java.time.LocalDate;
import java.util.Locale;

@Entity
@Table(name = "artist")
public class ArtistEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    @Basic(optional = false)
    private Locale country;

    private String artworkUrl;

    private String firstName;
    private LocalDate birthDate;
    private LocalDate deathDate;

    public ArtistDto toDto() {
        if (null == firstName && null == birthDate && null == deathDate) {
            return new BandArtistDto()
                .setId(id)
                .setName(name)
                .setCountry(country)
                .setArtworkUrl(artworkUrl);
        } else {
            return new PersonArtistDto()
                .setFirstName(firstName)
                .setBirthDate(birthDate)
                .setDeathDate(deathDate)
                .setId(id)
                .setName(name)
                .setCountry(country)
                .setArtworkUrl(artworkUrl);
        }
    }

    public Long getId() {
        return id;
    }

    public ArtistEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public ArtistEntity setName(final String name) {
        this.name = name;
        return this;
    }

    public Locale getCountry() {
        return country;
    }

    public ArtistEntity setCountry(final Locale country) {
        this.country = country;
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl;
    }

    public ArtistEntity setArtworkUrl(final String artworkUrl) {
        this.artworkUrl = artworkUrl;
        return this;
    }

    // ---------- Person Artist ---------------

    public String getFirstName() {
        return firstName;
    }

    public ArtistEntity setFirstName(final String firstName) {
        this.firstName = firstName;
        return this;
    }

    public LocalDate getBirthDate() {
        return birthDate;
    }

    public ArtistEntity setBirthDate(final LocalDate birthDate) {
        this.birthDate = birthDate;
        return this;
    }

    public LocalDate getDeathDate() {
        return deathDate;
    }

    public ArtistEntity setDeathDate(final LocalDate deathDate) {
        this.deathDate = deathDate;
        return this;
    }
}
