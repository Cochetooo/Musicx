package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

/**
 * Transferring album data throughout application layers.
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
