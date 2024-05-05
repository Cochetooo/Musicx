package fr.xahla.musicx.api.repository.searchCriterias;

public enum ArtistSearchCriterias {

    COUNTRY("country"),
    NAME("name");

    private final String column;

    ArtistSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
