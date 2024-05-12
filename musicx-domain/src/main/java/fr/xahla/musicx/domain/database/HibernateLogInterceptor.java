package fr.xahla.musicx.domain.database;

import org.hibernate.resource.jdbc.spi.StatementInspector;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class HibernateLogInterceptor implements StatementInspector {

    @Override public String inspect(final String sql) {
        logger().info("[Hibernate SQL] " + sql);
        return sql;
    }

}
