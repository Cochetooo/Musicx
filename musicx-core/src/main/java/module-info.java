module fr.xahla.musicx.core {

    // --- Musicx ---
    requires fr.xahla.musicx.api;

    // --- Logger ---
    requires java.logging;

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

    opens fr.xahla.musicx.core.model.data to org.hibernate.orm.core;
    opens fr.xahla.musicx.core.model.entity to org.hibernate.orm.core;

    // --- Exports ---

    exports fr.xahla.musicx.core.logging;
    exports fr.xahla.musicx.core.repository;
    exports fr.xahla.musicx.core.config;
    exports fr.xahla.musicx.core.model.data;

}