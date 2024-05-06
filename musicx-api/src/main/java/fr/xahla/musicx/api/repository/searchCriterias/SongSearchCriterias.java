package fr.xahla.musicx.api.repository.searchCriterias;

public enum SongSearchCriterias {

    ALBUM_NAME("album.name"),
    ARTIST_NAME("artist.name"),
    BIT_RATE("bit_rate"),
    DURATION("duration"),
    ID(""),
    PRIMARY_GENRE(""),
    SAMPLE_RATE("sample_rate"),
    SECONDARY_GENRE(""),
    TITLE("title");

    private final String column;

    SongSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
