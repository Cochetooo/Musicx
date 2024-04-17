package fr.xahla.musicx.desktop.views.menuBar;

import fr.xahla.musicx.desktop.DesktopApplication;
import fr.xahla.musicx.desktop.helper.FXMLHelper;
import javafx.application.Platform;
import javafx.collections.ListChangeListener;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.MenuItem;
import javafx.stage.Stage;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
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

    @FXML public MenuItem fileScanFoldersMenuItem;

    private ResourceBundle resourceBundle;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.fileScanFoldersMenuItem.setDisable(library().isEmpty());

        library().onFolderPathsChange(change
            -> Platform.runLater(() -> fileScanFoldersMenuItem.setDisable(change.getList().isEmpty())));
    }

    /* --------------------------- FILE --------------------------- */

    @FXML public void fileImportFolders() {
        FXMLHelper.showModal("importFolders.fxml", this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }

    @FXML public void fileScanFolders() {
        library().launchScanFoldersTask();
    }

    @FXML public void fileSettings() {
        FXMLHelper.showModal("settings.fxml", this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void fileShowConsole() {
        FXMLHelper.showModal("console.fxml", this.resourceBundle, resourceBundle.getString("console.title"));
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
