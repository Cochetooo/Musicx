package fr.xahla.musicx.api.repository.searchCriterias;

public enum AlbumSearchCriterias {

    ARTIST("artist_id"),
    CATALOG_NO("catalogNo"),
    ID("id"),
    LABEL("label_id"),
    NAME("name"),
    RELEASE_DATE("releaseDate");

    private final String column;

    AlbumSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
