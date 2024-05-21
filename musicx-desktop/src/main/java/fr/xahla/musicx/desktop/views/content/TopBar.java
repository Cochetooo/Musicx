package fr.xahla.musicx.desktop.views.content;

import atlantafx.base.controls.CustomTextField;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for search bar and tools.
 * @author Cochetooo
 * @since 0.3.3
 */
public class TopBar implements Initializable {

    @FXML private CustomTextField searchTextField;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        scene().bindSearchText(searchTextField.textProperty());
    }

}
