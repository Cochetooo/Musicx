package fr.xahla.musicx.domain.database;

public enum SqlOrderBy {

    ASCENDANT(""),
    DESCENDANT("DESC");

    private final String sqlOrderBy;

    SqlOrderBy(String sqlOperator) {
        this.sqlOrderBy = sqlOperator;
    }

    public String getSqlOrderBy() {
        return sqlOrderBy;
    }

}
