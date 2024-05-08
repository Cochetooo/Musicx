/** <b>Domain Module for Musicx apps, should be accessible for anyone that wants to make their own app
 * out of the raw contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
module fr.xahla.musicx.domain {

    // ------------- [REQUIRE] -------------

    // Musicx -> API
    requires transitive fr.xahla.musicx.api;

    // Java -> Logging
    requires java.logging;
    // Java -> Net
    requires java.net.http;

    // Lib -> Dotenv
    requires io.github.cdimascio.dotenv.java;

    // Lib -> JAudioTagger
    requires jaudiotagger;

    // Lib -> JSON
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
    requires org.hibernate.orm.community.dialects;

    // --- SQLite Database ---
    requires org.xerial.sqlitejdbc;

    // --- Opens ---

    opens fr.xahla.musicx.domain.model.entity to org.hibernate.orm.core;

    // ------------- [EXPORT] -------------
    exports fr.xahla.musicx.domain.application;
    exports fr.xahla.musicx.domain.helper;
    exports fr.xahla.musicx.domain.listener;
    exports fr.xahla.musicx.domain.manager;
    exports fr.xahla.musicx.domain.model.data;
    exports fr.xahla.musicx.domain.model.entity;
    exports fr.xahla.musicx.domain.model.enums;
    exports fr.xahla.musicx.domain.repository;
    exports fr.xahla.musicx.domain.service.localAudioFile;
    exports fr.xahla.musicx.domain.repository.data;
    exports fr.xahla.musicx.domain.helper.enums;

}