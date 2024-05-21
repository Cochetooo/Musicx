package fr.xahla.musicx.desktop.views.content.localLibrary;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Tab;
import javafx.scene.control.TabPane;

import java.net.URL;
import java.util.ResourceBundle;

/**
 * View for the content of the Local Library scene
 * @author Cochetooo
 * @since 0.3.3
 */
public class Content implements Initializable {

    @FXML private TabPane localLibraryTab;

    @FXML private Tab tabSongs;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        localLibraryTab.getSelectionModel().selectedItemProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue == tabSongs) {
                tabSongs.setContent(
                    FxmlHelper.getComponent(FxmlComponent.LOCAL_LIBRARY_SONGS.getFilepath(), resourceBundle)
                );
            }
        });
    }

}
