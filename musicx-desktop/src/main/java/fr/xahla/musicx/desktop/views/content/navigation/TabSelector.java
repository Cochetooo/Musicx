package fr.xahla.musicx.desktop.views.content.navigation;

import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TabPane;

import java.net.URL;
import java.util.ResourceBundle;

/**
 * @author Cochetooo
 * @since 0.3.3
 */
public class TabSelector implements Initializable {

    @FXML private TabPane pageTab;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        pageTab.selectionModelProperty().addListener((observable, oldValue, newValue) -> {

        });
    }

}
