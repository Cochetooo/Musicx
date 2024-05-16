package fr.xahla.musicx.domain.database;

import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.logging.LogMessage;

import java.util.HashMap;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.log;

/**
 * Build SQL queries for repositories.
 * @author Cochetooo
 * @since 0.3.1
 */
public final class QueryBuilder {

    /**
     * Stores data for a HQL query.
     * @since 0.3.1
     */
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
     * @since 0.3.1
     */
    public QueryBuilder.Query build() {
        if (null == clazz) {
            log(LogMessage.ERROR_QUERY_NO_TABLE_DEFINED, sql);
            return null;
        }

        return new Query(sql.toString(), parameters, clazz);
    }

    /* -------------- FROM -------------- */

    /**
     * @since 0.3.1
     */
    public QueryBuilder from(final Class<?> clazz) {
        sql
            .append("FROM ")
            .append(clazz.getSimpleName());

        this.clazz = clazz;

        return this;
    }

    /* -------------- WHERE -------------- */

    /**
     * @since 0.3.1
     */
    public QueryBuilder orWhere(final String column, final Object value) {
        return orWhere(column, value, SqlOperator.EQUAL);
    }

    /**
     * @since 0.3.1
     */
    public QueryBuilder orWhere(final String column, final Object value, final SqlOperator operator) {
        sql.append(" OR ")
            .append(column)
            .append(operator.getSQLOperator())
            .append(":").append(column);

        parameters.put(column, String.valueOf(value));

        return this;
    }

    /**
     * @since 0.3.1
     */
    public QueryBuilder where(final String column, final Object value) {
        return where(column, value, SqlOperator.EQUAL);
    }

    /**
     * @since 0.3.1
     */
    public QueryBuilder where(final String column, final Object value, final SqlOperator operator) {
        if (sql.toString().contains("WHERE")) {
            sql.append(" AND ");
        } else {
            sql.append(" WHERE ");
        }

        sql
            .append(column)
            .append(operator.getSQLOperator())
            .append(":").append(column);

        parameters.put(column, String.valueOf(value));

        return this;
    }

    /* -------------- WHERE IN -------------- */

    /**
     * @throws NullPointerException If class selector has not been set.
     * @since 0.3.2
     */
    public QueryBuilder whereIn(final String table, final String column, final Object value) {
        return whereIn(table, column, value, SqlOperator.EQUAL);
    }

    /**
     * @throws NullPointerException If class selector has not been set.
     * @since 0.3.2
     */
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

    /**
     * @since 0.3.1
     */
    public QueryBuilder orderBy(final String column, final SqlOrderBy orderBy) {
        sql
            .append(" ORDER BY ")
            .append(column)
            .append(orderBy);

        return this;
    }

}
