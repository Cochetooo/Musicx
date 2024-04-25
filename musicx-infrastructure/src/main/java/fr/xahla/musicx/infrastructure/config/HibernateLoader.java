package fr.xahla.musicx.infrastructure.config;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

/** <b>Class to load the Hibernate ORM and the H2 Database.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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
