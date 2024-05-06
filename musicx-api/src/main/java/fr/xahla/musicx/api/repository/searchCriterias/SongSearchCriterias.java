package fr.xahla.musicx.api.repository.searchCriterias;

public enum SongSearchCriterias {

    ALBUM_NAME("album.name"),
    ARTIST_NAME("artist.name"),
    BIT_RATE("bitRate"),
    DURATION("duration"),
    ID(""),
    PRIMARY_GENRE(""),
    SAMPLE_RATE("sampleRate"),
    SECONDARY_GENRE(""),
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
