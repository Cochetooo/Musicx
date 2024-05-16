package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.ArtistDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.util.List;
import java.util.Locale;

/**
 * Artist persistence class for database.
 * @author Cochetooo
 * @since 0.3.0
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

    /**
     * @since 0.3.0
     */
    public ArtistEntity fromDto(final ArtistDto artistDto) {
        this.setId(artistDto.getId());
        this.setName(artistDto.getName());
        this.setCountry(artistDto.getCountry());
        this.setArtworkUrl(artistDto.getArtworkUrl());

        return this;
    }

    /**
     * @since 0.3.0
     */
    public ArtistDto toDto() {
        return ArtistDto.builder()
            .id(id)
            .artworkUrl(artworkUrl)
            .country(country)
            .name(name)
            .build();
    }

}
