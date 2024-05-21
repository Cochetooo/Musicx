package fr.xahla.musicx.desktop.views.modal;

import atlantafx.base.controls.ToggleSwitch;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.settings;

/**
 * Modal for the application settings.
 * @author Cochetooo
 * @since 0.2.2
 */
public class Settings implements Initializable {

    @FXML private ToggleSwitch playerArtworkShadow;
    @FXML private ToggleSwitch playerBackgroundArtworkBind;
    @FXML private ToggleSwitch playerSmoothFadingStop;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        playerArtworkShadow.selectedProperty().bindBidirectional(settings().artworkShadowProperty());
        playerBackgroundArtworkBind.selectedProperty().bindBidirectional(settings().backgroundArtworkBindProperty());
        playerSmoothFadingStop.selectedProperty().bindBidirectional(settings().smoothPauseProperty());
    }
}
