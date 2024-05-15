package fr.xahla.musicx.domain.database;

import fr.xahla.musicx.domain.logging.LogMessage;
import org.hibernate.resource.jdbc.spi.StatementInspector;

import static fr.xahla.musicx.domain.application.AbstractContext.log;

/**
 * Intercepts SQL statements from Hibernate and logs them.
 * @author Cochetooo
 */
public class HibernateLogInterceptor implements StatementInspector {

    @Override public String inspect(final String sql) {
        log(LogMessage.INFO_HIBERNATE_SQL_STATEMENT, sql);
        return sql;
    }

}
