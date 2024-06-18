package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.model.enums.ReleaseType;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

/**
 * Transferring album data throughout application layers.
 * @author Cochetooo
 * @since 0.3.0
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
    private Short discTotal;
    private String name;
    private LocalDate releaseDate;
    private Short trackTotal;
    private ReleaseType type;

    private Byte rating;
    private Byte positiveRatingModel;
    private Character tier;

}
