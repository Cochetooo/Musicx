package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;

import java.util.Locale;

/**
 * Transferring artist data throughout application layers.
 * @author Cochetooo
 * @since 0.3.0
 */
@Data
@Builder
public class ArtistDto {

    private Long id;
    private String name;
    private Locale country;
    private String artworkUrl;

}
