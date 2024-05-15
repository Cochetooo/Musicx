package fr.xahla.musicx.domain.database;

import fr.xahla.musicx.domain.logging.LogMessage;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Loads configuration file for Hibernate ORM.
 * @author Cochetooo
 */
public final class HibernateLoader {

    private final SessionFactory sessionFactory;

    public static final HibernateLoader INSTANCE = new HibernateLoader();

    public HibernateLoader() {
        try {
            final var configuration = new Configuration().configure(
                HibernateLoader.class.getResource("hibernate.cfg.xml")
            );

            this.sessionFactory = configuration.buildSessionFactory();
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, LogMessage.ERROR_HIBERNATE_INITIALIZATION.msg(), exception);
            throw new ExceptionInInitializerError(exception);
        }
    }

    public SessionFactory getSession() {
        return sessionFactory;
    }

}
