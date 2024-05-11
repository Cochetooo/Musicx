package fr.xahla.musicx.api.repository.searchCriterias;

public enum SongSearchCriterias {

    ALBUM("album_id"),
    ARTIST("artist_id"),
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

    public String getColumn() {
        return column;
    }

}
