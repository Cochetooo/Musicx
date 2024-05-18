/**
 * Module management for Domain layer.
 * @author Cochetooo
 * @since 0.1.0
 */
module fr.xahla.musicx.domain {

    // ------------- [REQUIRE] -------------

    // Musicx -> API
    requires fr.xahla.musicx.api;

    // Java -> Logging
    requires java.logging;
    // Java -> Net
    requires java.net.http;

    // Lib -> Dotenv
    requires io.github.cdimascio.dotenv.java;

    // Lib -> Hibernate
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

    // Lib -> JAudioTagger
    requires jaudiotagger;

    // Lib -> JSON
    requires org.json;

    // Lib -> Lombok
    requires lombok;

    // Lib -> SQLite Database
    requires org.xerial.sqlitejdbc;
    requires org.jetbrains.annotations;

    // --- Opens ---

    opens fr.xahla.musicx.domain.model.entity to org.hibernate.orm.core;

    // --- Exports ---
    exports fr.xahla.musicx.domain.application;
    exports fr.xahla.musicx.domain.database;
    exports fr.xahla.musicx.domain.helper;
    exports fr.xahla.musicx.domain.helper.enums;
    exports fr.xahla.musicx.domain.listener;
    exports fr.xahla.musicx.domain.logging;
    exports fr.xahla.musicx.domain.model.data;
    exports fr.xahla.musicx.domain.model.entity;
    exports fr.xahla.musicx.domain.model.enums;
    exports fr.xahla.musicx.domain.repository;
    exports fr.xahla.musicx.domain.repository.data;
    exports fr.xahla.musicx.domain.service.apiHandler;
    exports fr.xahla.musicx.domain.service.importLocalSongs;
    exports fr.xahla.musicx.domain.service.saveLocalSongs;
    exports fr.xahla.musicx.domain.service.structureLocalSongs;

}