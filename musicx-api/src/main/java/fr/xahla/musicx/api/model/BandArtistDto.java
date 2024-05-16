package fr.xahla.musicx.api.model;

import lombok.*;

import java.util.List;
import java.util.Locale;

/**
 * Transferring band album data throughout application layers.
 * @author Cochetooo
 * @since 0.3.0
 */
@Getter
@Setter
public class BandArtistDto extends ArtistDto {

    private List<Long> memberIds;

    public BandArtistDto(final Long id, final String name, final Locale country, final String artworkUrl) {
        super(id, name, country, artworkUrl);
    }
}
