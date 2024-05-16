package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.util.List;
import java.util.Locale;

/**
 * Artist persistence class for database
 * @author Cochetooo
 */
@Entity
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(
    name = "artist_type",
    discriminatorType = DiscriminatorType.STRING
)
@Table(name = "artist")
@Getter
@Setter
public class ArtistEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String artworkUrl;

    @Basic(optional = false)
    private Locale country;

    private String name;

    public ArtistEntity fromDto(final ArtistDto artistDto) {
        this.setId(artistDto.getId());
        this.setName(artistDto.getName());
        this.setCountry(artistDto.getCountry());
        this.setArtworkUrl(artistDto.getArtworkUrl());

        return this;
    }

    public ArtistDto toDto() {
        return ArtistDto.builder()
            .id(id)
            .artworkUrl(artworkUrl)
            .country(country)
            .name(name)
            .build();
    }

}
