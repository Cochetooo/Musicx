package fr.xahla.musicx.desktop.views.modal;

import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TextArea;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.loggerManager;

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

    @FXML private TextArea consoleOutputTextArea;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        loggerManager().outputStreamProperty().addListener((observableValue, oldValue, newValue) -> {
            this.consoleOutputTextArea.setText(newValue.toString());
        });

        this.consoleOutputTextArea.setText(loggerManager().outputStreamProperty().toString());
    }
}
