package fr.xahla.musicx.desktop.views.modal;

import atlantafx.base.controls.ToggleSwitch;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

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
        playerArtworkShadow.selectedProperty().bindBidirectional(scene().getSettings().artworkShadowProperty());
        playerBackgroundArtworkBind.selectedProperty().bindBidirectional(scene().getSettings().backgroundArtworkBindProperty());
        playerSmoothFadingStop.selectedProperty().bindBidirectional(scene().getSettings().smoothPauseProperty());
    }
}
