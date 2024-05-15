package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

/**
 * Available criteria for filtering song search in database.
 * @author Cochetooo
 */
@Getter
public enum SongSearchCriteria {

    ALBUM("albumId"),
    ARTIST("artistId"),
    BIT_RATE("bitRate"),
    DURATION("duration"),
    ID("id"),
    SAMPLE_RATE("sampleRate"),
    TITLE("title"),
    TRACK_NUMBER("trackNumber");

    private final String column;

    SongSearchCriteria(final String column) {
        this.column = column;
    }

}
