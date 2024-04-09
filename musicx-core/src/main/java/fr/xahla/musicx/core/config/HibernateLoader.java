package fr.xahla.musicx.core.config;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

import java.io.File;

public final class HibernateLoader {

    private final SessionFactory sessionFactory;

    private static HibernateLoader hibernateLoader;

    public HibernateLoader() {
        try {
            var configuration = new Configuration().configure(
                HibernateLoader.class.getResource("hibernate.cfg.xml")
            );
            this.sessionFactory = configuration.buildSessionFactory();
        } catch (Exception ex) {
            System.err.println("Initial SessionFactory creation failed: " + ex);
            throw new ExceptionInInitializerError(ex);
        }
    }

    public static void setHibernateLoader(final HibernateLoader _hibernateLoader) {
        hibernateLoader = _hibernateLoader;
    }

    public static Session openSession() {
        return hibernateLoader.sessionFactory.openSession();
    }

}
