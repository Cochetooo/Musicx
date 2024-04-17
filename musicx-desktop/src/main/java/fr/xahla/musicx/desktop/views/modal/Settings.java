package fr.xahla.musicx.desktop.views.modal;

import atlantafx.base.controls.ToggleSwitch;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.settings;

/** <b>Modal for the app settings.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Settings implements Initializable {

    @FXML private ToggleSwitch playerSmoothFadingStop;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        settings().smoothFadeStopProperty().bind(playerSmoothFadingStop.selectedProperty());
    }
}
