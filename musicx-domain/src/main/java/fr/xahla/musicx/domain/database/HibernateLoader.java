package fr.xahla.musicx.domain.database;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

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
            logger().log(Level.SEVERE, "Couldn't initialize Hibernate and its session factory.", exception);
            throw new ExceptionInInitializerError(exception);
        }
    }

    public static Session openSession() {
        return INSTANCE.sessionFactory.openSession();
    }

}
