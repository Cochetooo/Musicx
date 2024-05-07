package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.data.ArtistInterface;
import lombok.*;

import java.util.Locale;

/** <b>(API) Interface for Artist Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
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
