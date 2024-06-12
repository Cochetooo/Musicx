package fr.xahla.musicx.desktop.views.pages.library.search;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.desktop.model.entity.*;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ListView;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class LibrarySearch implements Initializable {

    @FXML private CustomTextField searchTextField;

    @FXML private Button onlineModeButton;

    @FXML private ListView<Song> searchTrackResultList;
    @FXML private ListView<Album> searchAlbumResultList;
    @FXML private ListView<Artist> searchArtistResultList;
    @FXML private ListView<Label> searchLabelResultList;
    @FXML private ListView<Genre> searchGenreResultList;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {

    }

    private void bindTextFields() {
        scene().bindSearchText(searchTextField.textProperty());
    }

    @FXML private void toggleOnlineMode(final ActionEvent actionEvent) {

    }
}
