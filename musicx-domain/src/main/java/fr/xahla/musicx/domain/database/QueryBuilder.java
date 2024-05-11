package fr.xahla.musicx.domain.database;

import java.util.HashMap;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

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

    public QueryBuilder.Query build() {
        if (null == clazz) {
            logger().severe("No table has been defined for query: " + sql);
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

    /* -------------- ORDER BY -------------- */

    public QueryBuilder orderBy(final String column, final SqlOrderBy orderBy) {
        sql
            .append(" ORDER BY ")
            .append(column)
            .append(orderBy);

        return this;
    }

}
