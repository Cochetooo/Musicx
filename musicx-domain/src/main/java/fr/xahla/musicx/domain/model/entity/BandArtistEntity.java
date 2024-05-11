package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.BandArtistDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;

@Entity
@DiscriminatorValue("BAND")
@Getter
@Setter
public class BandArtistEntity extends ArtistEntity {

    @ManyToMany
    @JoinTable(
        name = "band_members",
        joinColumns = @JoinColumn(name = "band_id"),
        inverseJoinColumns = @JoinColumn(name = "person_id")
    )
    private List<PersonArtistEntity> members;

    @Override public BandArtistEntity fromDto(final ArtistDto artistDto) {
        super.fromDto(artistDto);

        if (artistDto instanceof final BandArtistDto bandArtistDto) {
            if (null != bandArtistDto.getMemberIds()) {
                members = new ArrayList<>();
                Hibernate.initialize(members);
                members.clear();

                bandArtistDto.getMemberIds().forEach((artistId) -> {
                    final var artist = new PersonArtistEntity();
                    artist.setId(artistId);
                    members.add(artist);
                });
            }
        }

        return this;
    }

    @Override public BandArtistDto toDto() {
        final var bandArtistDto = (BandArtistDto) super.toDto();

        if (null != members && Hibernate.isInitialized(members)) {
            bandArtistDto.setMemberIds(members.stream()
                .map(PersonArtistEntity::getId)
                .toList()
            );
        }

        return bandArtistDto;
    }

}
