package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.repository.*;
import fr.xahla.musicx.domain.helper.ArrayHelper;
import fr.xahla.musicx.domain.helper.FileHelper;
import fr.xahla.musicx.domain.listener.ProgressListener;
import org.jaudiotagger.audio.AudioFileIO;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public final class ImportSongsFromFolders {

    /**
     * Scan all folders from the library and hydrate its songs collection from all the files found.<br>
     * Every files that has not correctly been read is ignored.
     *
     * @param folderPaths List of folder paths to search songs for
     * @param acceptedFormats An array of file extension as string
     * @param progressListener A listener interface to get the current progress.
     */
    public void execute(
        final List<String> folderPaths,
        final List<String> acceptedFormats,
        final ProgressListener progressListener
    ) {
        final var startTime = System.currentTimeMillis();

        // Retrieve audio files from all the library folder paths
        final var audioFiles = getAudioFiles(folderPaths, acceptedFormats);

        final var fileNumbers = audioFiles.size();

        if (0 == fileNumbers) {
            logger().info("No audio files has been found in the chosen folders.");
            progressListener.updateProgress(1, 1);
            return;
        }

        // Initialize progress, counter and collection
        progressListener.updateProgress(0, fileNumbers);
        final var fileProgressCount = new AtomicInteger();

        // Generate a thread pool to allow multithreading for the song collection hydration.
        try (final var threadExecutor = Executors.newFixedThreadPool(1)) {
            // Loop on each audio files
            audioFiles.forEach(filepath -> {
                try {
                    final var audioFile = AudioFileIO.read(filepath.toFile());
                    // Create a response from a new thread that try to create a song and all its object members from the audio file.
                    threadExecutor.submit(() -> new PersistAudioFileMetadata().execute(audioFile));
                } catch (final Exception exception) {
                    logger().log(Level.SEVERE, "Couldn't read audio file properly: " + filepath, exception);
                }

                // Increment and update the progress.
                fileProgressCount.getAndIncrement();
                progressListener.updateProgress(fileProgressCount.get(), fileNumbers);
            });
        } finally {
            progressListener.updateProgress(1, 1);

            logger().info("Finished importing " + fileProgressCount.get() + " audio files in " +
                (System.currentTimeMillis() - startTime) + " ms");
        }
    }

    private List<Path> getAudioFiles(
        final List<String> paths,
        final List<String> acceptedFormats
    ) {
        final var audioFiles = new ArrayList<Path>();

        for (final var path : paths) {
            final var directory = Paths.get(path);

            try (final var fileStream = Files.walk(directory)) {
                audioFiles.addAll(fileStream.filter(filepath -> {
                    if (!filepath.toFile().isFile()) {
                        return false;
                    }

                    final var extension = FileHelper.getExtensionFromPath(filepath);

                    return ArrayHelper.inArrayStringIgnoreCase(extension.toLowerCase(), acceptedFormats);
                }).toList());
            } catch (final IOException exception) {
                logger().log(Level.SEVERE, "Failed to walk through directory: " + directory.toAbsolutePath(), exception);
            }
        }

        return audioFiles;
    }

}
