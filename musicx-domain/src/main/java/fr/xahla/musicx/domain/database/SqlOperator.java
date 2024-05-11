package fr.xahla.musicx.domain.database;

public enum SqlOperator {

    EQUAL("="),
    NOT_EQUAL("<>"),
    GREATER_THAN(">"),
    LESS_THAN("<"),
    GREATER_THAN_OR_EQUAL(">="),
    LESS_THAN_OR_EQUAL("<="),
    LIKE("LIKE"),
    IN("IN"),
    NOT_IN("NOT IN"),
    IS_NULL("IS NULL"),
    IS_NOT_NULL("IS NOT NULL");

    private final String sqlOperator;

    SqlOperator(String sqlOperator) {
        this.sqlOperator = sqlOperator;
    }

    public String getSQLOperator() {
        return sqlOperator;
    }
}
