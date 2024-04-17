package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.core.repository.LibraryRepository;
import fr.xahla.musicx.core.repository.SongRepository;
import fr.xahla.musicx.desktop.listener.ProgressListener;
import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.TaskProgress;
import fr.xahla.musicx.desktop.model.data.LocalSong;
import fr.xahla.musicx.desktop.model.entity.Library;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.service.GetAudioFilesFromFoldersService;

import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.concurrent.Task;
import org.jaudiotagger.audio.AudioFileIO;

import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.atomic.AtomicInteger;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.settings;
import static fr.xahla.musicx.desktop.DesktopContext.taskProgress;

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
public class LibraryManager {

    private final LibraryRepository libraryRepository;
    private final SongRepository songRepository;
    private final Library library;

    public LibraryManager() {
        this.library = new Library().setName("MyLibrary");

        this.libraryRepository = new LibraryRepository();
        this.songRepository = new SongRepository();

        final var existingLibrary = this.libraryRepository.findOneByName("MyLibrary");

        if (null != existingLibrary) {
            this.library.set(existingLibrary);
        }
    }

    public void launchScanFoldersTask() {
        final var scanFolderTask = new Task<>() {
            @Override public Void call() {
                scanFoldersForLibrary(
                    settings().getScanLibraryAudioFormats(),
                    this::updateProgress
                );

                save();

                return null;
            }
        };

        taskProgress().setTaskProgress(
            new TaskProgress(scanFolderTask, "library.scanFolderProgress")
        );

        new Thread(scanFolderTask).start();
    }

    /**
     * Scan all folders from the library and hydrate its songs collection from all the files found.<br>
     * Every files that has not correctly been read is ignored.
     *
     * @param acceptedFormats An array of file extension as string
     * @param progressListener A listener interface to get the current progress.
     */
    public void scanFoldersForLibrary(
        final List<String> acceptedFormats,
        final ProgressListener progressListener
    ) {
        final var startTime = System.currentTimeMillis();

        final var paths = this.library.getFolderPaths();

        // Retrieve audio files from all the library folder paths
        final var audioFiles = GetAudioFilesFromFoldersService.execute(
            paths,
            acceptedFormats
        );

        final var fileNumbers = audioFiles.size();

        if (fileNumbers == 0) {
            logger().info("No audio files has been found in the chosen folders.");
            progressListener.updateProgress(1, 1);
            return;
        }

        // Initialize progress, counter and collection
        progressListener.updateProgress(0, fileNumbers);
        final var fileProgressCount = new AtomicInteger();
        final var futureSongs = new ArrayList<Future<Song>>();

        // Generate a thread pool to allow multithreading for the song collection hydration.
        try (final var threadExecutor = Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors())) {
            // Loop on each audio files
            audioFiles.forEach(filepath -> {
                try {
                    final var audioFile = AudioFileIO.read(filepath.toFile());
                    final var localSong = new LocalSong(audioFile);

                    // Create a response from a new thread that try to create a song and all its object members from the LocalSong.
                    futureSongs.add(threadExecutor.submit(() -> this.getFromLocalSong(localSong, filepath.toAbsolutePath().toString())));

                    // Increment and update the progress.
                    fileProgressCount.getAndIncrement();
                    progressListener.updateProgress(fileProgressCount.get(), fileNumbers);
                } catch (final Exception exception) {
                    logger().severe(exception.getLocalizedMessage());
                }
            });
        } finally {
            final var songs = new ArrayList<Song>();

            futureSongs.forEach((futureSong) -> {
                try {
                    songs.add(futureSong.get());
                } catch (final InterruptedException | ExecutionException e) {
                    logger().severe(e.getLocalizedMessage());
                }
            });

            // When all is done, hydrate library with the songs collected
            this.library.setSongs(songs);

            progressListener.updateProgress(1, 1);

            logger().info("Library scan made in " + (System.currentTimeMillis() - startTime) + "ms.");
        }
    }

    private Song getFromLocalSong(final LocalSong localSong, final String filepath) {
        final var domainSong = songRepository.getFromLocalSong(localSong);

        if (null != domainSong) {
            return new Song().set(domainSong).setFilePath(filepath);
        }

        return null;
    }

    /**
     * Clear the song collection and the folder paths of the library,
     * then update the database.
     */
    public void clear() {
        this.library.setSongs(new ArrayList<Song>());
        this.library.setFolderPaths(new ArrayList<>());
        this.save();
    }

    /**
     * Persist the library in the database.
     */
    public void save() {
        this.libraryRepository.save(library);
    }

    public boolean isEmpty() {
        if (null == this.library.getFolderPaths()) {
            return true;
        }

        return this.library.getFolderPaths().isEmpty();
    }

    public void addFolder(final File directory) {
        if (!directory.isDirectory()) {
            logger().warning(ErrorMessage.FOLDER_PATH_NOT_DIRECTORY.getMsg(directory.getAbsolutePath()));
            return;
        }

        library.getFolderPaths().add(directory.getAbsolutePath());

        this.save();
    }

    // --- Accessors / Modifiers ---

    public ObservableList<String> getFolderPaths() {
        return library.getFolderPaths();
    }

    public String getName() {
        return library.getName();
    }

    public ObservableList<Song> getSongs() {
        return library.getSongs();
    }

    // --- Event / Listeners ---

    public void onFolderPathsChange(final ListChangeListener<String> change) {
        this.library.folderPathsProperty().addListener(change);
    }

    public void onSongsChange(final ListChangeListener<Song> change) {
        this.library.songsProperty().addListener(change);
    }
}
