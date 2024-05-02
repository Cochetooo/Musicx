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
    requires org.hibernate.orm.community.dialects;

    // --- SQLite Database ---
    requires org.xerial.sqlitejdbc;

    // --- Opens ---

    opens fr.xahla.musicx.infrastructure.local.model to org.hibernate.orm.core;

}