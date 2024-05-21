package fr.xahla.musicx.desktop.views.content;

import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the content grid layout.
 * @author Cochetooo
 * @since 0.2.0
 */
public class ContentLayout implements Initializable {

    @FXML private HBox contentSceneBox;
    @FXML private VBox rightNavContainer;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        scene().getSceneContent().onChange((oldValue, newValue) -> {
            contentSceneBox.getChildren().setAll(newValue);
        });

        scene().getRightNavContent().onChange((oldValue, newValue) -> {
            if (null == newValue) {
                rightNavContainer.getChildren().clear();
            } else {
                rightNavContainer.getChildren().setAll(newValue);
            }
        });
    }
}
