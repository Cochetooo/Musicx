package fr.xahla.musicx.domain.helper;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.database.HibernateLoader.openSession;

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
        try (final var session = openSession()){
            final var result = session.createQuery(query, clazz);

            parameters.forEach(result::setParameter);

            return result.list();
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "An exception has occured while trying to query: " + query, exception);
            return new ArrayList<>();
        }
    }

}
