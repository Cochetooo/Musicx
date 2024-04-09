package fr.xahla.musicx.desktop.views.content.navigation;

import fr.xahla.musicx.desktop.model.entity.Artist;
import javafx.application.Platform;
import javafx.beans.property.StringProperty;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.util.Callback;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.*;

public class SelectorList implements Initializable {

    @FXML private ListView<Artist> selectorListView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.selectorListView.setItems(artist().getArtists());

        this.selectorListView.setCellFactory(list -> {
            final var cell = new ListCell<Artist>() {
                @Override public void updateItem(final Artist artist, final boolean empty) {
                    super.updateItem(artist, empty);

                    if (empty || null == artist) {
                        setText("");
                    } else {
                        setText(artist.getName());
                    }
                }
            };

            return cell;
        });

        this.selectorListView.setStyle(
            "-fx-font-weight: bold"
        );
    }

    @FXML private void onListClick(final MouseEvent event) {
        final var artist = this.selectorListView.getSelectionModel().getSelectedItem();

        if (MouseButton.PRIMARY == event.getButton()) {
            trackList().setQueue(
                artist().getSongsFromArtist(library().getSongs(), artist),
                0
            );
        }
    }
}
