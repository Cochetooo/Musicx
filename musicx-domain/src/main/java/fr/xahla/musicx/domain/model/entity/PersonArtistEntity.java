package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.PersonArtistDto;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.ManyToMany;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

/**
 * Person Artist persistence class for database.
 * @author Cochetooo
 * @since 0.3.0
 */
@Entity
@DiscriminatorValue("PERSON")
@Getter
@Setter
public class PersonArtistEntity extends ArtistEntity {

    private String firstName;

    private LocalDate birthDate;

    private LocalDate deathDate;

    @ManyToMany(mappedBy = "members")
    private List<BandArtistEntity> bands;

    // Casts

    /**
     * @since 0.3.0
     */
    public PersonArtistEntity fromDto(final PersonArtistDto artistDto) {
        super.fromDto(artistDto);

        if (artistDto instanceof final PersonArtistDto personArtistDto) {
            this.setFirstName(personArtistDto.getFirstName());
            this.setBirthDate(personArtistDto.getBirthDate());
            this.setDeathDate(personArtistDto.getDeathDate());

            if (null != personArtistDto.getBandIds()) {
                bands = new ArrayList<>();
                Hibernate.initialize(bands);
                bands.clear();

                personArtistDto.getBandIds().forEach((artistId) -> {
                    final var artist = new BandArtistEntity();
                    artist.setId(artistId);
                    bands.add(artist);
                });
            }
        }

        return this;
    }

    /**
     * @since 0.3.0
     */
    public PersonArtistDto toDto() {
        final var personArtistDto = (PersonArtistDto) super.toDto();

        personArtistDto.setFirstName(firstName);
        personArtistDto.setBirthDate(birthDate);
        personArtistDto.setDeathDate(deathDate);

        if (null != bands && Hibernate.isInitialized(bands)) {
            personArtistDto.setBandIds(bands.stream()
                .map(BandArtistEntity::getId)
                .toList()
            );
        }

        return personArtistDto;
    }
}
