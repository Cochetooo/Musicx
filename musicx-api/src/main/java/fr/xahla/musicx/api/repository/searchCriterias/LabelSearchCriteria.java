package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

/**
 * Available criteria for filtering label search in database.
 * @author Cochetooo
 */
@Getter
public enum LabelSearchCriteria {

    ID("id"),
    NAME("name");

    private final String column;

    LabelSearchCriteria(final String column) {
        this.column = column;
    }

}
