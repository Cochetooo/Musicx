/** <b>Desktop Module for Musicx, defining the Desktop Application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
module fr.xahla.musicx.desktop {

    // --- Musicx ---
    requires fr.xahla.musicx.api;
    requires fr.xahla.musicx.domain;
    requires fr.xahla.musicx.infrastructure;

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

    // --- JAudioTagger ---
    requires jaudiotagger;

    // -- Opens --
    opens fr.xahla.musicx.desktop to javafx.graphics, javafx.fxml;
    opens fr.xahla.musicx.desktop.views to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.content to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.content.navigation to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.modal to javafx.fxml;
    opens fr.xahla.musicx.desktop.views.menuBar to javafx.fxml;

    opens fr.xahla.musicx.desktop.model.entity to javafx.base;
}