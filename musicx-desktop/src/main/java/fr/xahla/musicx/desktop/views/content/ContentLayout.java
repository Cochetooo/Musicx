package fr.xahla.musicx.desktop.views.content;

import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.layout.VBox;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.rightNavContent;

/**
 * View for the content grid layout.
 * @author Cochetooo
 * @since 0.2.0
 */
public class ContentLayout implements Initializable {

    @FXML private VBox rightNavContainer;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        rightNavContent().onChange((oldValue, newValue) -> {
            if (null == newValue) {
                rightNavContainer.getChildren().clear();
            } else {
                rightNavContainer.getChildren().add(rightNavContent().get());
            }
        });
    }
}
