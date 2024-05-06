package fr.xahla.musicx.api.repository.searchCriterias;

public enum LabelSearchCriterias {

    ALBUM_NAME(""),
    GENRE(""),
    ID("id"),
    NAME("name");

    private final String column;

    LabelSearchCriterias(final String column) {
        this.column = column;
    }

    public String getColumn() {
        return column;
    }

}
