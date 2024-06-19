package fr.xahla.musicx.desktop.views.modals.selector;

import fr.xahla.musicx.desktop.model.entity.Artist;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.scene.control.*;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.Priority;
import javafx.util.Callback;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

/**
 * @author Cochetooo
 * @since 0.5.1
 */
public class ArtistSelector {

    private final Dialog<Artist> dialog;
    private final ComboBox<Artist> artistComboBox;
    private final ObservableList<Artist> artists;

    public ArtistSelector(final List<Artist> artistList) {
        dialog = new Dialog<>();
        dialog.setTitle("Select Artist");
        dialog.setHeaderText("Please select an artist from the list:");

        dialog.getDialogPane().getButtonTypes().addAll(ButtonType.OK, ButtonType.CANCEL);

        artists = FXCollections.observableList(new ArrayList<>(artistList));
        artistComboBox = new ComboBox<>(artists);
        artistComboBox.setEditable(true);
        artistComboBox.setCellFactory(listView -> new ListCell<>() {
            @Override public void updateItem(final Artist artist, final boolean empty) {
                super.updateItem(artist, empty);
                setText(empty ? "" : artist.getName());
            }
        });

        // Setup auto-completion
        final var editor = artistComboBox.getEditor();
        editor.textProperty().addListener((obs, oldText, newText) -> {
            if (!newText.isEmpty()) {
                artistComboBox.show();
                ObservableList<Artist> filteredList = FXCollections.observableArrayList();
                for (final var artist : artists) {
                    if (artist.getName().toLowerCase().contains(newText.toLowerCase())) {
                        filteredList.add(artist);
                    }
                }
                artistComboBox.setItems(filteredList);
            } else {
                artistComboBox.hide();
                artistComboBox.setItems(artists);
            }
        });

        // Layout the dialog
        final var grid = new GridPane();
        grid.setHgap(10);
        grid.setVgap(10);
        grid.add(new Label("Artist:"), 0, 0);
        grid.add(artistComboBox, 1, 0);
        GridPane.setHgrow(artistComboBox, Priority.ALWAYS);

        dialog.getDialogPane().setContent(grid);

        // Convert the result to the artist name when the OK button is clicked
        dialog.setResultConverter(dialogButton -> {
            if (dialogButton == ButtonType.OK) {
                return artistComboBox.getValue();
            }
            return null;
        });
    }

    public Optional<Artist> showAndWait() {
        return dialog.showAndWait();
    }

}
