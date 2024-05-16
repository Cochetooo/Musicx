package fr.xahla.musicx.domain.helper;

import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.database.QueryResponse;
import fr.xahla.musicx.domain.logging.LogMessage;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.application.AbstractContext.openSession;

/**
 * Utility class for queries
 * @author Cochetooo
 */
public final class QueryHelper {

    @Deprecated public static List<?> query_find_by_criteria(
        final Class<?> clazz,
        final Map<String, Object> criteria
    ) {
        var sql = new StringBuilder("FROM " + clazz.getSimpleName());

        if (!criteria.isEmpty()) {
            sql.append(" WHERE ");
        }

        sql.append(criteria.entrySet().stream()
            .map(entry -> {
                if (entry.getValue() instanceof CharSequence) {
                    return entry.getKey() + " LIKE :" + entry.getKey();
                } else {
                    return entry.getKey() + " = :" + entry.getKey();
                }
            })
            .collect(Collectors.joining(" AND "))
        );

        return query(clazz, sql.toString(), criteria); // @TODO
    }

    /**
     * @param clazz The class entity to fetch data
     * @return A collection of object for an entity
     */
    public static List<?> query_find_all(final Class<?> clazz) {
        return query(
            new QueryBuilder()
                .from(clazz)
                .build()
        ).response();
    }

    @Deprecated private static List<?> query(
        final Class<?> clazz,
        final String query,
        final Map<String, Object> parameters
    ) {
        try (final var session = openSession()){
            final var result = session.createQuery(query, clazz);

            parameters.forEach(result::setParameter);

            return result.list();
        } catch (final Exception exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_QUERY.msg(), query),
                exception
            );

            return new ArrayList<>();
        }
    }

    /**
     * @return A QueryResponse from the query, if the statement is not valid it will return an empty QueryResponse.
     */
    public static QueryResponse query(final QueryBuilder.Query query) {
        try (final var session = openSession()) {
            final var result = session.createQuery(query.request(), query.clazz());
            query.parameters().forEach(result::setParameter);

            return new QueryResponse(result.list());
        } catch (final Exception exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_QUERY.msg(), query),
                exception
            );

            return new QueryResponse(List.of());
        }
    }

}
