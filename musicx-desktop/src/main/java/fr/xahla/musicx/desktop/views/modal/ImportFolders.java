package fr.xahla.musicx.desktop.views.modal;

import javafx.application.Platform;
import javafx.beans.property.StringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.*;
import javafx.stage.DirectoryChooser;
import org.controlsfx.control.CheckListView;

import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.*;

/** <b>View for the library folder imports management modal.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
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

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.clearButton.setDisable(library().isEmpty());
        this.scanFoldersButton.setDisable(library().isEmpty());

        this.formatButtons = new ArrayList<>(List.of(
            formatMp3Button, formatFlacButton, formatOggButton, formatWavButton, formatM4aButton
        ));

        this.formatButtons.forEach(toggle
            -> toggle.selectedProperty().addListener(change -> {
                settings().getScanLibraryAudioFormats().setAll(this.getSelectedFormats());
            }
        ));

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
            trackList().clear();
            player().clearQueue();
            artist().getArtists().clear();
        }
    }

    private List<String> getSelectedFormats() {
        return this.formatButtons.stream()
            .filter(ToggleButton::isSelected)
            .map(ToggleButton::getText)
            .collect(Collectors.toList());
    }
}
