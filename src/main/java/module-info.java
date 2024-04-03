module fr.xahla.musicx {
    requires javafx.controls;
    requires javafx.fxml;
    requires javafx.web;
    requires javafx.media;

    requires org.kordamp.ikonli.fontawesome5;

    requires org.jboss.logging;
    requires jakarta.cdi;
    requires jakarta.persistence;
    requires jakarta.transaction;
    requires jakarta.xml.bind;
    requires com.fasterxml.classmate;
    requires org.hibernate.commons.annotations;
    requires net.bytebuddy;

    requires org.controlsfx.controls;
    requires com.dlsc.formsfx;
    requires org.kordamp.ikonli.javafx;
    requires org.kordamp.bootstrapfx.core;
    requires eu.hansolo.tilesfx;
    requires jaudiotagger;
    requires org.json;
    requires atlantafx.base;
    requires com.h2database;
    requires org.hibernate.orm.core;

    requires java.naming;

    opens fr.xahla.musicx.desktop.model to javafx.base;
    opens fr.xahla.musicx.infrastructure.persistence.entity to org.hibernate.orm.core;

    opens fr.xahla.musicx to javafx.fxml;

    exports fr.xahla.musicx;
    exports fr.xahla.musicx.desktop.application;
    exports fr.xahla.musicx.domain.service;
    exports fr.xahla.musicx.desktop.views;

    opens fr.xahla.musicx.desktop.application to javafx.fxml;
    opens fr.xahla.musicx.desktop.views to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.content.topbar to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.content.trackList to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.footer.appInfo to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.footer.audioPlayer to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.footer.audioPlayer.trackControls to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.footer.audioPlayer.trackInfo to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menubar to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menubar.submenu.file to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menubar.submenu.controls to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.content.trackList.dialog to javafx.fxml;
}