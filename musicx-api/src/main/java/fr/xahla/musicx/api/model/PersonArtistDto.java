package fr.xahla.musicx.api.model;

import java.time.LocalDate;

public class PersonArtistDto extends ArtistDto {

    private String firstName;
    private LocalDate birthDate;
    private LocalDate deathDate;

    public String getFirstName() {
        return firstName;
    }

    public PersonArtistDto setFirstName(final String firstName) {
        this.firstName = firstName;
        return this;
    }

    public LocalDate getBirthDate() {
        return birthDate;
    }

    public PersonArtistDto setBirthDate(final LocalDate birthDate) {
        this.birthDate = birthDate;
        return this;
    }

    public LocalDate getDeathDate() {
        return deathDate;
    }

    public PersonArtistDto setDeathDate(final LocalDate deathDate) {
        this.deathDate = deathDate;
        return this;
    }

}
