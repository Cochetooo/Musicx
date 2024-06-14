package fr.xahla.musicx.desktop.views.menus.submenus;

import fr.xahla.musicx.desktop.model.TaskProgress;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.service.normalizeAudio.NormalizeAllSongs;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class ToolsMenu implements Initializable {

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {

    }

    @FXML private void normalizeSongsVolume() {
        final var normalizeSongsVolumeTask = new Task<>() {
            @Override protected Void call() {
                new NormalizeAllSongs().execute(
                    scene().getLocalLibraryScene().getLibrary().getLocalSongs().stream()
                        .map(Song::getDto)
                        .toList(),
                    0.0,
                    this::updateProgress
                );

                return null;
            }
        };

        scene().getTaskProgress().setTaskProgress(
            new TaskProgress(normalizeSongsVolumeTask, "tools.normalizeLibrary")
        );

        new Thread(normalizeSongsVolumeTask).start();
    }
}
