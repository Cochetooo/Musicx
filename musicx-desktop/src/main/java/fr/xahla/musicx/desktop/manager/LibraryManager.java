package fr.xahla.musicx.desktop.manager;

/** <b>Class that allow views to use Library model, while keeping a protection layer to its usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
@Deprecated
public class LibraryManager {

    /* private final ListProperty<Song> songs;
    private final ListProperty<String> folderPaths;

    private final List<SongEditListener> songEditListeners;

    public LibraryManager() {
        songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        folderPaths = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        songEditListeners = new ArrayList<>();

        this.refresh();
    }

    public void launchScanFoldersTask() {
        final var scanFolderTask = new Task<>() {
            @Override public Void call() {
                new ImportSongsFromFolders().execute(
                    folderPaths,
                    settings().getScanLibraryAudioFormats(),
                    this::updateProgress
                );

                library().refresh();

                return null;
            }
        };

        taskProgress().setTaskProgress(
            new TaskProgress(scanFolderTask, "library.scanFolderProgress")
        );

        new Thread(scanFolderTask).start();
    } */

    /**
     * Clear the song collection and the folder paths of the library,
     * then update the database.
     */
    public void clear() {
        throw new RuntimeException("TODO");
    }

    /*
    public void refresh() {
        songs.clear();
        final var songsDto = songRepository().findAll();

        songsDto.forEach(songDto -> songs.add(new Song(songDto)));
    }

    public boolean isEmpty() {
        return this.songs.isEmpty();
    }

    public void addFolder(final File directory) {
        if (!directory.isDirectory()) {
            logger().warn(directory.getAbsolutePath() + " is not a directory.");
            return;
        }

        folderPaths.add(directory.getAbsolutePath());
    } */

    /**
     * @since 0.3.3
     *
    public void editSong(final Song song) {
        final var index = songs.indexOf(song);

        songEditListeners.forEach(listener -> listener.onSongEdited(song, index));
    }

    // --- Accessors / Modifiers ---

    public ObservableList<String> getFolderPaths() {
        return folderPaths;
    }

    public ObservableList<Song> getSongs() {
        return songs;
    }

    // --- Event / Listeners ---

    public void onFolderPathsChange(final ListChangeListener<String> change) {
        this.folderPaths.addListener(change);
    }

    public void onSongsChange(final ListChangeListener<Song> change) {
        this.songs.addListener(change);
    }

    /**
     * @since 0.3.3
     *
    public void onSongEdited(final SongEditListener listener) {
        songEditListeners.add(listener);
    }

    // --- Inner classes ---

    /**
     * @since 0.3.3
     *
    interface SongEditListener {
        void onSongEdited(final Song song, final int position);
    } */

}
