package fr.xahla.musicx.api.model;

import lombok.*;

import java.util.Locale;

/**
 * Transferring artist data throughout application layers.
 * @author Cochetooo
 */
@Data
@Builder
public class ArtistDto {

    private Long id;
    private String name;
    private Locale country;
    private String artworkUrl;

}
