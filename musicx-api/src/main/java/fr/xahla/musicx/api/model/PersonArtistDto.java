package fr.xahla.musicx.api.model;

import lombok.*;

import java.time.LocalDate;
import java.util.List;
import java.util.Locale;

/**
 * Transferring person artist data throughout application layers.
 * @author Cochetooo
 * @since 0.3.0
 */
@Getter
@Setter
public class PersonArtistDto extends ArtistDto {

    private List<Long> bandIds;

    private String firstName;
    private LocalDate birthDate;
    private LocalDate deathDate;

    public PersonArtistDto(final Long id, final String name, final Locale country, final String artworkUrl) {
        super(id, name, country, artworkUrl);
    }
}
