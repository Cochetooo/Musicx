package fr.xahla.musicx.desktop.views.modal;

import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibrary;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.ButtonType;
import javafx.scene.control.ToggleButton;
import javafx.stage.DirectoryChooser;
import org.controlsfx.control.CheckListView;

import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.*;

/**
 * View for the library folder imports management modal.
 * @author Cochetooo
 * @since 0.2.0
 */
public class ImportFolders implements Initializable {

    @FXML private CheckListView<String> folderPathsCheckListView;
    @FXML private Button clearButton;
    @FXML private Button scanFoldersButton;

    @FXML private ToggleButton formatMp3Button;
    @FXML private ToggleButton formatFlacButton;
    @FXML private ToggleButton formatOggButton;
    @FXML private ToggleButton formatWavButton;
    @FXML private ToggleButton formatM4aButton;

    private List<ToggleButton> formatButtons;

    private ResourceBundle resourceBundle;
    private LocalLibrary library;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
        this.library = scene().getLocalLibraryScene().getLibrary();

        this.clearButton.setDisable(library.isEmpty());
        this.scanFoldersButton.setDisable(library.isEmpty());

        this.formatButtons = new ArrayList<>(List.of(
            formatMp3Button, formatFlacButton, formatOggButton, formatWavButton, formatM4aButton
        ));

        this.formatButtons.forEach(toggle
            -> toggle.selectedProperty().addListener(change -> {
                scene().getSettings().getScanLibraryAudioFormats().setAll(this.getSelectedFormats());
            }
        ));

        this.folderPathsCheckListView.setItems(FXCollections.observableList(
            new ArrayList<>(library.getFolders())
        ));

        library.onFoldersChange(change -> {
            config().setLocalFolders(library.getFolders());

            Platform.runLater(() -> {
                folderPathsCheckListView.getItems().setAll(change.getList());
                clearButton.setDisable(change.getList().isEmpty());
                scanFoldersButton.setDisable(change.getList().isEmpty());
            });
        });
    }

    @FXML public void addFolder() {
        final var directoryChooser = new DirectoryChooser();
        final var selectedDirectory = directoryChooser.showDialog(null);

        if (null != selectedDirectory) {
            library.addFolder(selectedDirectory);
        }
    }

    @FXML public void scanFolders() {
        library.scanFolders();
    }

    @FXML public void clear() {
        final var alert = new Alert(Alert.AlertType.CONFIRMATION);
        alert.setTitle(resourceBundle.getString("importFolders.confirmClear.title"));
        alert.setContentText(resourceBundle.getString("importFolders.confirmClear.content"));

        final var response = alert.showAndWait();

        if (response.isEmpty()) {
            return;
        }

        if (response.get() == ButtonType.OK) {
            library.clear();
            audioPlayer().clearQueue();
        }
    }

    private List<String> getSelectedFormats() {
        return this.formatButtons.stream()
            .filter(ToggleButton::isSelected)
            .map(ToggleButton::getText)
            .collect(Collectors.toList());
    }
}
