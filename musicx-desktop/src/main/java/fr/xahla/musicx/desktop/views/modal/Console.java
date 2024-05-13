package fr.xahla.musicx.desktop.views.modal;

import fr.xahla.musicx.domain.logging.ConsoleType;
import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.paint.Color;
import javafx.scene.text.Text;
import javafx.scene.text.TextFlow;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.logging.Level;

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

    @FXML private TextFlow queryOutputTextArea;
    @FXML private TextFlow consoleOutputTextArea;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var consoleHandler = (SplitConsoleHandler) logger().getHandlers()[0];

        consoleHandler.getOtherLogs().forEach((log) -> addText(log, Level.FINE, ConsoleType.OTHER));
        consoleHandler.getHibernateSqlLogs().forEach((log) -> addText(log, Level.FINE, ConsoleType.HIBERNATE));

        consoleHandler.addListener(this::addText);
    }

    private void addText(final String message, final Level level, final ConsoleType type) {
        final var text = new Text(message);

        if (Level.SEVERE == level) text.setFill(Color.SALMON);
        else if (Level.WARNING == level) text.setFill(Color.GOLD);
        else if (Level.INFO == level) text.setFill(Color.LIGHTCYAN);
        else if (Level.CONFIG == level) text.setFill(Color.WHITE);
        else text.setFill(Color.LIGHTGRAY);

        if (ConsoleType.HIBERNATE == type) queryOutputTextArea.getChildren().add(text);
        else consoleOutputTextArea.getChildren().add(text);
    }
}
