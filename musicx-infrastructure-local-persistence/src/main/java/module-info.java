module fr.xahla.musicx.infrastructure.local {

    requires fr.xahla.musicx.api;
    requires fr.xahla.musicx.domain;

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

    opens fr.xahla.musicx.infrastructure.local.model to org.hibernate.orm.core;

}