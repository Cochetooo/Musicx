package fr.xahla.musicx.desktop.context.scene.localLibrary;

import fr.xahla.musicx.desktop.helper.CollectionSortHelper;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.desktop.model.TaskProgress;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.service.deleteLocalLibrary.DeleteLocalLibrary;
import fr.xahla.musicx.domain.service.importLocalSongs.ImportSongsFromFolders;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.concurrent.Task;
import javafx.scene.control.ButtonType;

import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import static fr.xahla.musicx.desktop.context.DesktopContext.config;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * Contains Local Library data.
 * @author Cochetooo
 * @since 0.4.0
 */
public final class LocalLibrary {

    private final ListProperty<Song> localSongs;
    private final ListProperty<String> folders;

    public LocalLibrary() {
        this.localSongs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.folders = new SimpleListProperty<>(FXCollections.observableList(
            new ArrayList<>(config().getLocalFolders())
        ));

        this.refresh();
    }

    public void clear() {
        final var response = FxmlHelper.showConfirmAlert(
            "Delete Local Library",
            "Are you sure you want to delete the whole locale library?"
        );

        if (response.isPresent() && ButtonType.OK == response.get()) {
            final var result = new DeleteLocalLibrary().execute();

            FxmlHelper.showInformationAlert(
                "Delete Success",
                result.entrySet().stream().map(entry -> entry.getKey() + ": " + entry.getValue() + " deleted").collect(Collectors.joining("\n"))
            );

            refresh();
        }
    }

    /**
     * Clear all songs from local library, then find all songs from the repository
     * and hydrate the song list.
     * @since 0.3.3
     */
    public void refresh() {
        localSongs.clear();
        final var songsDto = songRepository().findAll();

        songsDto.forEach(songDto -> localSongs.add(new Song(songDto)));
        CollectionSortHelper.sortSongsByYears(localSongs);
    }

    public void scanFolders() {
        final var scanFolderTask = new Task<>() {
            @Override public Void call() {
                new ImportSongsFromFolders().execute(
                    folders,
                    scene().getSettings().getScanLibraryAudioFormats(),
                    this::updateProgress
                );

                refresh();

                return null;
            }
        };

        scene().getTaskProgress().setTaskProgress(
            new TaskProgress(scanFolderTask, "library.scanFolderProgress")
        );

        new Thread(scanFolderTask).start();
    }

    // --- Getters / Setters ---

    public void addFolder(final File directory) {
        if (!directory.isDirectory()) {
            logger().warn("IO_FILE_NOT_DIRECTORY", directory.getAbsolutePath());
            return;
        }

        folders.add(directory.getAbsolutePath());
    }

    public ObservableList<String> getFolders() {
        return folders;
    }

    public ObservableList<Song> getLocalSongs() {
        return localSongs;
    }

    public void setLocalSongs(final List<Song> localSongs) {
        this.localSongs.setAll(localSongs);
    }

    public boolean isEmpty() {
        return this.localSongs.isEmpty();
    }

    // --- Listeners ---

    public void onLocalSongsChange(final ListChangeListener<Song> change) {
        localSongs.addListener(change);
    }

    public void onFoldersChange(final ListChangeListener<String> change) {
        folders.addListener(change);
    }

}
