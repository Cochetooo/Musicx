package fr.xahla.musicx.desktop.views;

import fr.xahla.musicx.desktop.config.FxmlComponent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.config;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * Main view for the desktop application.
 * @author Cochetooo
 * @since 0.1.0
 */
public class Application implements Initializable {

    @FXML public VBox applicationBox;

    @FXML private HBox contentSceneBox;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        scene().getSceneContent().onChange((oldValue, newValue)
            -> contentSceneBox.getChildren().setAll(newValue));

        scene().getSceneContent().switchContent(
            FxmlComponent.valueOf("SCENE_" + config().getActiveScene()));
    }
}
