package fr.xahla.musicx.desktop.views.modal;

import javafx.application.Platform;
import javafx.beans.property.StringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.ButtonType;
import javafx.stage.DirectoryChooser;
import org.controlsfx.control.CheckListView;

import java.net.URL;
import java.util.ArrayList;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.library;

public class ImportFolders implements Initializable {

    @FXML private CheckListView<String> folderPathsCheckListView;
    @FXML private Button clearButton;
    @FXML private Button scanFoldersButton;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.clearButton.setDisable(library().isEmpty());
        this.scanFoldersButton.setDisable(library().isEmpty());

        this.folderPathsCheckListView.setItems(FXCollections.observableList(
            new ArrayList<>(library().getFolderPaths())
        ));

        library().onFolderPathsChange(change -> {
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
            library().addFolder(selectedDirectory);
        }
    }

    @FXML public void scanFolders() {
        library().launchScanFoldersTask();
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
            library().clear();
        }
    }
}
