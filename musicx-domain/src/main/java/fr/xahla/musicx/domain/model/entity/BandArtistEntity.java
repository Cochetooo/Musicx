package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.BandArtistDto;
import fr.xahla.musicx.api.model.data.BandArtistInterface;
import fr.xahla.musicx.api.model.data.PersonArtistInterface;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;

@Entity
@DiscriminatorValue("BAND")
public class BandArtistEntity extends ArtistEntity implements BandArtistInterface {

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
        final var bandArtistDto = new BandArtistDto();

        bandArtistDto
            .setId(this.getId())
            .setArtworkUrl(this.getArtworkUrl())
            .setCountry(this.getCountry())
            .setName(this.getName());

        if (null != members && Hibernate.isInitialized(members)) {
            bandArtistDto.setMemberIds(members.stream()
                .map(PersonArtistEntity::getId)
                .toList()
            );
        }

        return bandArtistDto;
    }

    @Override public List<PersonArtistEntity> getMembers() {
        return members;
    }

    @Override public BandArtistEntity setMembers(final List<? extends PersonArtistInterface> artists) {
        this.members = artists.stream()
            .filter(PersonArtistEntity.class::isInstance)
            .map(PersonArtistEntity.class::cast)
            .toList();

        return this;
    }

    @Override public List<BandArtistEntity> getRelatedBands() {
        return List.of();
    }

    @Override public BandArtistEntity setRelatedBands(final List<? extends BandArtistInterface> relatedBands) {
        return null;
    }

}
