package fr.xahla.musicx.config.db;

import org.hibernate.HibernateException;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

import java.io.File;

public class HibernateLoader {

    private final SessionFactory sessionFactory;
    private final String configurationPath = "src/main/resources/fr/xahla/musicx/config/hibernate.cfg.xml";

    public HibernateLoader() {
        try {
            var configuration = new Configuration().configure(
                new File(configurationPath)
            );
            this.sessionFactory = configuration.buildSessionFactory();
        } catch (Exception ex) {
            System.err.println("Initial SessionFactory creation failed: " + ex);
            throw new ExceptionInInitializerError(ex);
        }
    }

    public SessionFactory getSessionFactory() {
        return this.sessionFactory;
    }

}
