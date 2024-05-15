package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

/**
 * Available criteria for filtering genre search in database.
 * @author Cochetooo
 */
@Getter
public enum GenreSearchCriteria {

    ID("id"),
    NAME("name");

    private final String column;

    GenreSearchCriteria(final String column) {
        this.column = column;
    }

}
