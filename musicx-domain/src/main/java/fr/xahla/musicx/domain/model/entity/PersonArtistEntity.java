package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.PersonArtistDto;
import fr.xahla.musicx.api.model.data.BandArtistInterface;
import fr.xahla.musicx.api.model.data.PersonArtistInterface;
import org.hibernate.Hibernate;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@Entity
@DiscriminatorValue("PERSON")
public class PersonArtistEntity extends ArtistEntity implements PersonArtistInterface {

    private String firstName;

    private LocalDate birthDate;

    private LocalDate deathDate;

    @ManyToMany(mappedBy = "members")
    private List<BandArtistEntity> bands;

    @Override public PersonArtistEntity fromDto(final ArtistDto artistDto) {
        super.fromDto(artistDto);

        if (artistDto instanceof final PersonArtistDto personArtistDto) {
            this.setFirstName(personArtistDto.getFirstName())
                .setBirthDate(personArtistDto.getBirthDate())
                .setDeathDate(personArtistDto.getDeathDate());

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

    @Override public PersonArtistDto toDto() {
        final var personArtistDto = new PersonArtistDto();

        personArtistDto
            .setFirstName(firstName)
            .setBirthDate(birthDate)
            .setDeathDate(deathDate)
            .setName(this.getName())
            .setCountry(this.getCountry())
            .setArtworkUrl(this.getArtworkUrl())
            .setId(this.getId());

        if (null != bands && Hibernate.isInitialized(bands)) {
            personArtistDto.setBandIds(bands.stream()
                .map(BandArtistEntity::getId)
                .toList()
            );
        }

        return personArtistDto;
    }

    @Override public String getFirstName() {
        return firstName;
    }

    @Override public PersonArtistEntity setFirstName(final String firstName) {
        this.firstName = firstName;
        return this;
    }

    @Override public LocalDate getBirthDate() {
        return birthDate;
    }

    @Override public PersonArtistEntity setBirthDate(final LocalDate birthDate) {
        this.birthDate = birthDate;
        return this;
    }

    @Override public LocalDate getDeathDate() {
        return deathDate;
    }

    @Override public PersonArtistEntity setDeathDate(final LocalDate deathDate) {
        this.deathDate = deathDate;
        return this;
    }

    @Override public List<BandArtistEntity> getBands() {
        return bands;
    }

    @Override public PersonArtistEntity setBands(final List<? extends BandArtistInterface> bands) {
        this.bands = bands.stream()
            .filter(BandArtistEntity.class::isInstance)
            .map(BandArtistEntity.class::cast)
            .toList();

        return this;
    }
}
