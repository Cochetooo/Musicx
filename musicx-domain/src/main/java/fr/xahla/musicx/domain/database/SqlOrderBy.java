package fr.xahla.musicx.domain.database;

import lombok.Getter;

/**
 * List of all orders types for a query.
 * @author Cochetooo
 */
@Getter
public enum SqlOrderBy {

    ASCENDANT(""),
    DESCENDANT("DESC");

    private final String sqlOrderBy;

    SqlOrderBy(String sqlOperator) {
        this.sqlOrderBy = sqlOperator;
    }

}
