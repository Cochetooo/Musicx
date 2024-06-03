package fr.xahla.musicx.desktop.views.menus;

import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibrary;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.domain.service.structureLocalSongs.StructureAudioFilesTree;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.MenuItem;
import javafx.stage.DirectoryChooser;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the top menu bar and all its sub-menus.
 * @author Cochetooo
 * @since 0.2.0
 */
public class MainMenu implements Initializable {

    @FXML private MenuItem fileScanFoldersMenuItem;

    private ResourceBundle resourceBundle;
    private LocalLibrary library;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
        this.library = scene().getLocalLibraryScene().getLibrary();

        this.fileScanFoldersMenuItem.setDisable(library.getFolders().isEmpty());

        library.onFoldersChange(change
            -> Platform.runLater(() -> fileScanFoldersMenuItem.setDisable(change.getList().isEmpty())));
    }

    /* --------------------------- FILE --------------------------- */

    @FXML public void fileImportFolders() {
        FxmlHelper.showModal(FxmlComponent.MODAL_IMPORT_FOLDERS, this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }

    @FXML public void fileScanFolders() {
        library.scanFolders();
    }

    @FXML public void fileStructureFolders() {
        final var folderChooser = new DirectoryChooser();
        final var folder = folderChooser.showDialog(null);

        if (null != folder) {
            new StructureAudioFilesTree().execute(folder.getAbsolutePath());
        }
    }

    @FXML public void fileSettings() {
        FxmlHelper.showModal(FxmlComponent.SCENE_SETTINGS, this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void fileShowConsole() {
        FxmlHelper.showModal(FxmlComponent.SCENE_CONSOLE, this.resourceBundle, resourceBundle.getString("console.title"));
    }

    @FXML public void fileExit() {
        Platform.exit();
    }

    /* --------------------------- PLAYER --------------------------- */
    @FXML public void playerTogglePlaying() {
        audioPlayer().togglePlaying();
    }

    @FXML public void playerPrevious() {
        audioPlayer().previous();
    }

    @FXML public void playerNext() {
        audioPlayer().next();
    }

    @FXML public void playerMute() {
        audioPlayer().mute();
    }
}
