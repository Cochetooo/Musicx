package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

/** <b>(API) Interface for Album Model contracts.</b>
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
public class AlbumDto {

    private Long id;

    private Long artistId;
    private Map<Long, ArtistRole> creditArtistIds;
    private Long labelId;
    private List<Long> primaryGenreIds;
    private List<Long> secondaryGenreIds;

    private String artworkUrl;
    private String catalogNo;
    private short discTotal;
    private String name;
    private LocalDate releaseDate;
    private short trackTotal;
    private ReleaseType type;

}
