package fr.xahla.musicx.api.repository.searchCriterias;

public enum AlbumSearchCriterias {

    ARTIST_NAME("artist.name"),
    NAME("name"),
    PRIMARY_GENRE(""),
    RELEASE_DATE("release_date"),
    SECONDARY_GENRE("");

    private String column;

    AlbumSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
