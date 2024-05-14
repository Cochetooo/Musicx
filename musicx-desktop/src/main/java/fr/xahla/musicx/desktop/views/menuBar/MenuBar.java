package fr.xahla.musicx.desktop.views.menuBar;

import fr.xahla.musicx.desktop.helper.FXMLHelper;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.domain.service.localAudioFile.StructureAudioFilesTree;
import javafx.application.Platform;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.MenuItem;
import javafx.stage.DirectoryChooser;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.library;
import static fr.xahla.musicx.desktop.DesktopContext.player;

/** <b>View for the top menu bar and all its sub-menus.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class MenuBar implements Initializable {

    @FXML private MenuItem fileScanFoldersMenuItem;
    @FXML private MenuItem fileStructureFolders;

    private ResourceBundle resourceBundle;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.fileScanFoldersMenuItem.setDisable(library().isEmpty());

        library().onFolderPathsChange(change
            -> Platform.runLater(() -> fileScanFoldersMenuItem.setDisable(change.getList().isEmpty())));
    }

    /* --------------------------- FILE --------------------------- */

    @FXML public void fileImportFolders() {
        FXMLHelper.showModal(FxmlComponent.MODAL_IMPORT_FOLDERS, this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }

    @FXML public void fileScanFolders() {
        library().launchScanFoldersTask();
    }

    @FXML public void fileStructureFolders() {
        final var folderChooser = new DirectoryChooser();
        final var folder = folderChooser.showDialog(null);

        if (null != folder) {
            new StructureAudioFilesTree().execute(folder.getAbsolutePath());
        }
    }

    @FXML public void fileSettings() {
        FXMLHelper.showModal(FxmlComponent.MODAL_SETTINGS, this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void fileShowConsole() {
        FXMLHelper.showModal(FxmlComponent.MODAL_CONSOLE, this.resourceBundle, resourceBundle.getString("console.title"));
    }

    @FXML public void fileExit() {
        Platform.exit();
    }

    /* --------------------------- PLAYER --------------------------- */
    @FXML public void playerTogglePlaying() {
        player().togglePlaying();
    }

    @FXML public void playerPrevious() {
        player().previous();
    }

    @FXML public void playerNext() {
        player().next();
    }

    @FXML public void playerMute() {
        player().mute();
    }
}
