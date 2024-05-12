package fr.xahla.musicx.desktop.views.modal;

import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TextArea;

import java.net.URL;
import java.util.ArrayList;
import java.util.ResourceBundle;
import java.util.logging.Handler;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/** <b>View for the console output.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Console implements Initializable {

    @FXML private TextArea queryOutputTextArea;
    @FXML private TextArea consoleOutputTextArea;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var consoleHandler = (SplitConsoleHandler) logger().getHandlers()[0];

        consoleOutputTextArea.setText(String.join("\n", consoleHandler.getOtherLogs()));
        queryOutputTextArea.setText(String.join("\n", consoleHandler.getHibernateSqlLogs()));

        consoleHandler.addListener((message) -> {
            consoleOutputTextArea.setText(String.join("\n", consoleHandler.getOtherLogs()));
            queryOutputTextArea.setText(String.join("\n", consoleHandler.getHibernateSqlLogs()));
        });
    }
}
