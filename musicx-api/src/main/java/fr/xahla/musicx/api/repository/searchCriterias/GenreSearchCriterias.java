package fr.xahla.musicx.api.repository.searchCriterias;

public enum GenreSearchCriterias {

    ID("id"),
    NAME("name");

    private final String column;

    GenreSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
