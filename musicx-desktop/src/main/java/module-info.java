/**
 * Module management for desktop application Front-End.
 * @author Cochetooo
 * @since 0.1.0
 */
module fr.xahla.musicx.desktop {

    // --- Musicx ---
    requires fr.xahla.musicx.api;
    requires fr.xahla.musicx.domain;

    // --- Logging ---
    requires java.logging;

    // --- JavaFX ---
    requires javafx.controls;
    requires javafx.fxml;
    requires javafx.media;
    requires javafx.web;

    // --- Ikonli ---
    requires org.kordamp.ikonli.fluentui;
    requires org.kordamp.ikonli.javafx;

    // --- ControlsFX ---
    requires org.controlsfx.controls;

    // --- FormsFX ---
    requires com.dlsc.formsfx;

    // --- BootstrapFX ---
    requires org.kordamp.bootstrapfx.core;

    // --- TilesFX ---
    requires eu.hansolo.tilesfx;

    // --- AtlantaFX ---
    requires atlantafx.base;

    // --- JFoenix ---
    requires com.jfoenix;

    // --- JSON ---
    requires org.json;

    // -- Opens --
    opens fr.xahla.musicx.desktop to javafx.graphics, javafx.fxml;
    opens fr.xahla.musicx.desktop.context to javafx.fxml, javafx.graphics;
    opens fr.xahla.musicx.desktop.model.entity to javafx.base;

    opens fr.xahla.musicx.desktop.views to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.audioPlayer to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menus to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menus.submenus to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.modals.console to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.modals.localFolders to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.navigation to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.pages.editor.genre to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.pages.library.search to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.pages.library.songs to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.pages.settings to javafx.fxml;
}