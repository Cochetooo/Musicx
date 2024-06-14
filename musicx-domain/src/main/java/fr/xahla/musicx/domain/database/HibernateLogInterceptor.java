package fr.xahla.musicx.domain.database;

import org.hibernate.resource.jdbc.spi.StatementInspector;

/**
 * Intercepts SQL statements from Hibernate and logs them.
 * @author Cochetooo
 * @since 0.3.1
 */
public class HibernateLogInterceptor implements StatementInspector {

    @Override public String inspect(final String sql) {
        //logger().info("HIBERNATE_QUERY", sql);
        return sql;
    }

}
