/** <b>Core Module for Musicx, should be accessible for anyone that develops its own Musicx application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
module fr.xahla.musicx.infrastructure {

    // --- Musicx ---
    requires fr.xahla.musicx.api;

    // --- Logger ---
    requires java.logging;

    // --- HTTP Client ---
    requires java.net.http;

    // --- JSON ---
    requires org.json;

    // --- Hibernate ---
    requires org.jboss.logging;
    requires jakarta.cdi;
    requires jakarta.persistence;
    requires jakarta.transaction;
    requires jakarta.xml.bind;
    requires com.fasterxml.classmate;
    requires net.bytebuddy;

    requires org.hibernate.commons.annotations;
    requires org.hibernate.orm.core;

    // --- H2 Database ---
    requires com.h2database;

    // --- Opens ---

    opens fr.xahla.musicx.infrastructure.model.data to org.hibernate.orm.core;
    opens fr.xahla.musicx.infrastructure.model.entity to org.hibernate.orm.core;

    // --- Exports ---

    exports fr.xahla.musicx.infrastructure.repository;
    exports fr.xahla.musicx.infrastructure.config;
    exports fr.xahla.musicx.infrastructure.model.data;
    exports fr.xahla.musicx.infrastructure.service.albumArtwork;
    exports fr.xahla.musicx.infrastructure.model.data.enums;
    exports fr.xahla.musicx.infrastructure.model;

}