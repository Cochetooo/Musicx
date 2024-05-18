package fr.xahla.musicx.domain.database;

import org.hibernate.resource.jdbc.spi.StatementInspector;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Intercepts SQL statements from Hibernate and logs them.
 * @author Cochetooo
 * @since 0.3.1
 */
public class HibernateLogInterceptor implements StatementInspector {

    @Override public String inspect(final String sql) {
        logger().info("HIBERNATE_QUERY", sql);
        return sql;
    }

}
