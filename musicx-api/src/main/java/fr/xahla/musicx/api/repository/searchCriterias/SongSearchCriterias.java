package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

@Getter
public enum SongSearchCriterias {

    ALBUM("albumId"),
    ARTIST("artistId"),
    BIT_RATE("bitRate"),
    DURATION("duration"),
    ID("id"),
    SAMPLE_RATE("sampleRate"),
    TITLE("title"),
    TRACK_NUMBER("trackNumber");

    private final String column;

    SongSearchCriterias(final String column) {
        this.column = column;
    }

}
