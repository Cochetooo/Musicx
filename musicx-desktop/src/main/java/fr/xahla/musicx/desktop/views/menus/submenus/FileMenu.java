package fr.xahla.musicx.desktop.views.menus.submenus;

import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibrary;
import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.domain.service.structureLocalSongs.StructureAudioFilesTree;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.MenuItem;
import javafx.stage.DirectoryChooser;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class FileMenu implements Initializable {

    @FXML private MenuItem scanLocalFoldersMenuItem;

    private ResourceBundle resourceBundle;
    private LocalLibrary library;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
        this.library = scene().getLocalLibraryScene().getLibrary();

        this.scanLocalFoldersMenuItem.setDisable(library.getFolders().isEmpty());

        library.onFoldersChange(change
            -> Platform.runLater(() -> scanLocalFoldersMenuItem.setDisable(change.getList().isEmpty())));
    }

    /* --------------------------- FILE --------------------------- */

    @FXML public void openLocalFoldersModal() {
        FxmlHelper.showModal(FxmlComponent.MODAL_LOCAL_FOLDERS, "importFolders.title");
    }

    @FXML public void scanLocalFolders() {
        library.scanFolders();
    }

    @FXML public void structureFolders() {
        final var folderChooser = new DirectoryChooser();
        final var folder = folderChooser.showDialog(null);

        if (null != folder) {
            new StructureAudioFilesTree().execute(folder.getAbsolutePath());

            FxmlHelper.showInformationAlert("File restructure", "Files have been moved successfully to a new structure: " + folder.getAbsolutePath());
        }
    }

    @FXML public void openSettings() {
        FxmlHelper.showModal(FxmlComponent.SCENE_SETTINGS, "settings.title");
    }

    @FXML public void showConsole() {
        FxmlHelper.showModal(FxmlComponent.MODAL_CONSOLE, "console.title");
    }

    @FXML public void exitProgram() {
        Platform.exit();
    }

}
