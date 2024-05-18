package fr.xahla.musicx.domain.database;

import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Loads configuration file for Hibernate ORM.
 * @author Cochetooo
 * @since 0.1.0
 */
public final class HibernateLoader {

    private final SessionFactory sessionFactory;

    /**
     * @throws ExceptionInInitializerError If Hibernate has not been initialized correctly.
     */
    public HibernateLoader() {
        try {
            final var configuration = new Configuration().configure(
                HibernateLoader.class.getResource("hibernate.cfg.xml")
            );

            this.sessionFactory = configuration.buildSessionFactory();
        } catch (final Exception exception) {
            logger().error(exception, "HIBERNATE_INITIALIZATION_ERROR");
            throw exception;
        }
    }

    public SessionFactory getSession() {
        return sessionFactory;
    }

}
