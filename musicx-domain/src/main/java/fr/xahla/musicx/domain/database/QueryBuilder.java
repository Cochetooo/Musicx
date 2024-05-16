package fr.xahla.musicx.domain.database;

import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.logging.LogMessage;

import java.util.HashMap;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.log;

/**
 * Build SQL queries for repositories.
 * @author Cochetooo
 */
public final class QueryBuilder {

    public record Query(
        String request,
        Map<String, String> parameters,
        Class<?> clazz
    ) {}

    private final StringBuilder sql;
    private Class<?> clazz;
    private final Map<String, String> parameters;

    public QueryBuilder() {
        sql = new StringBuilder();
        parameters = new HashMap<>();
    }

    /**
     * @return A query instance, or <b>null</b> if no class selector has been queried.
     */
    public QueryBuilder.Query build() {
        if (null == clazz) {
            log(LogMessage.ERROR_QUERY_NO_TABLE_DEFINED, sql);
            return null;
        }

        return new Query(sql.toString(), parameters, clazz);
    }

    /* -------------- FROM -------------- */

    public QueryBuilder from(final Class<?> clazz) {
        sql
            .append("FROM ")
            .append(clazz.getSimpleName());

        this.clazz = clazz;

        return this;
    }

    /* -------------- WHERE -------------- */

    public QueryBuilder andWhere(final String column, final Object value) {
        return andWhere(column, value, SqlOperator.EQUAL);
    }

    public QueryBuilder andWhere(final String column, final Object value, final SqlOperator operator) {
        sql.append(" AND ");
        return where(column, value, operator);
    }

    public QueryBuilder orWhere(final String column, final Object value) {
        return orWhere(column, value, SqlOperator.EQUAL);
    }

    public QueryBuilder orWhere(final String column, final Object value, final SqlOperator operator) {
        sql.append(" OR ");
        return where(column, value, operator);
    }

    public QueryBuilder where(final String column, final Object value) {
        return where(column, value, SqlOperator.EQUAL);
    }

    public QueryBuilder where(final String column, final Object value, final SqlOperator operator) {
        sql
            .append(" WHERE ")
            .append(column)
            .append(operator.getSQLOperator())
            .append(":").append(column);

        parameters.put(column, String.valueOf(value));

        return this;
    }

    /* -------------- WHERE IN -------------- */

    public QueryBuilder whereIn(final String table, final String column, final Object value) {
        return whereIn(table, column, value, SqlOperator.EQUAL);
    }

    public QueryBuilder whereIn(final String table, final String column, final Object value, final SqlOperator operator) {
        if (null == clazz) {
            log(LogMessage.ERROR_QUERY_NO_TABLE_DEFINED, sql);
            throw new NullPointerException();
        }

        final var pivotTable = clazz.getSimpleName() + "_" + table;
        final var mergedColumnName = table + StringHelper.str_uc_first(column);

        sql
            .append(" JOIN ").append(pivotTable)
            .append(" ON ").append(clazz.getSimpleName()).append(".id = ")
            .append(pivotTable).append(".").append(clazz.getSimpleName()).append("_id")
            .append(" JOIN ").append(table)
            .append(" ON ").append(pivotTable).append(".").append(table).append("_id = ")
            .append(table).append(".id")
            .append(" WHERE ").append(table).append(".").append(column)
            .append(operator.getSQLOperator())
            .append(":").append(mergedColumnName);

        parameters.put(mergedColumnName, String.valueOf(value));

        return this;
    }

    /* -------------- ORDER BY -------------- */

    public QueryBuilder orderBy(final String column, final SqlOrderBy orderBy) {
        sql
            .append(" ORDER BY ")
            .append(column)
            .append(orderBy);

        return this;
    }

}
