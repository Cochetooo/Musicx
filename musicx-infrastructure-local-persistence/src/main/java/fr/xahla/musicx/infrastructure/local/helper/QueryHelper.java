package fr.xahla.musicx.infrastructure.local.helper;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;

public final class QueryHelper {

    public static List<?> findByCriteria(
        final Class<?> clazz,
        final Map<String, Object> criteria
    ) {
        var sql = new StringBuilder("FROM " + clazz.getSimpleName());

        if (!criteria.isEmpty()) {
            sql.append(" WHERE ");
        }

        sql.append(criteria.entrySet().stream()
            .map(entry -> entry.getKey() + " = :" + entry.getKey())
            .collect(Collectors.joining(" AND "))
        );

        return query(clazz, sql.toString(), criteria); // @TODO
    }

    public static List<?> findAll(final Class<?> clazz) {
        return query(
            clazz,
            "FROM " + clazz.getSimpleName(),
            Map.of()
        );
    }

    private static List<?> query(
        final Class<?> clazz,
        final String query,
        final Map<String, Object> parameters
    ) {
        logger().info("SQL Request: " + query);

        try (final var session = openSession()){
            final var result = session.createQuery(query, clazz);

            parameters.forEach(result::setParameter);

            return result.list();
        }
    }

}
