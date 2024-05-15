package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

/**
 * An album is made by an artist and contains a list of songs.<br>
 * It has a name, a release date, a number
 * It can be released by a music label and can have credited artists, with their respective roles.<br>
 * An album has primary and secondary genres.<br>
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
