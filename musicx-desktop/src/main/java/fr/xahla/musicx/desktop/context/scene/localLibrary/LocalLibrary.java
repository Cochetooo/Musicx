package fr.xahla.musicx.desktop.context.scene.localLibrary;

import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.desktop.context.DesktopContext.config;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * Contains Local Library data.
 * @author Cochetooo
 * @since 0.3.3
 */
public final class LocalLibrary {

    private final ListProperty<Song> localSongs;
    private final ListProperty<String> folders;

    public LocalLibrary() {
        this.localSongs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.folders = new SimpleListProperty<>(FXCollections.observableList(config().getLocalFolders()));
    }

    public void clear() {
        throw new RuntimeException("TODO");
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

}
