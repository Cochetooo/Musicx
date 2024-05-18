package fr.xahla.musicx.domain.service.importLocalSongs;

import fr.xahla.musicx.domain.helper.ArrayHelper;
import fr.xahla.musicx.domain.helper.Benchmark;
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

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Collect all audio files from a source folder and import all valid songs to the database.
 * @author Cochetooo
 * @since 0.3.0
 */
public final class ImportSongsFromFolders {

    /**
     * Scan all folders from the library and hydrate its songs collection from all the files found.<br>
     * Every files that has not correctly been read is ignored.
     *
     * @param folderPaths List of folder paths to search songs for
     * @param acceptedFormats An array of file extension as string
     * @param progressListener A listener interface to get the current progress.
     *
     * @since 0.3.0
     */
    public void execute(
        final List<String> folderPaths,
        final List<String> acceptedFormats,
        final ProgressListener progressListener
    ) {
        final var benchmark = new Benchmark();

        // Retrieve audio files from all the library folder paths
        final var audioFiles = getAudioFiles(folderPaths, acceptedFormats);

        final var fileNumbers = audioFiles.size();

        if (0 == fileNumbers) {
            logger().info("IO_FILE_FOUND", "0");
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
                    logger().error(exception, "AUDIO_TAGGER_READ_FILE_ERROR", filepath);
                }

                // Increment and update the progress.
                fileProgressCount.getAndIncrement();
                progressListener.updateProgress(fileProgressCount.get(), fileNumbers);
            });
        } finally {
            progressListener.updateProgress(1, 1);

            benchmark.print("Finished importing " + fileProgressCount.get() + " audio files");
        }
    }

    /**
     * @since 0.3.0
     */
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

                    final var extension = FileHelper.file_get_extension(filepath);

                    return ArrayHelper.array_in_string_ignore_case(extension.toLowerCase(), acceptedFormats);
                }).toList());
            } catch (final IOException exception) {
                logger().error(exception, "IO_FOLDER_BROWSE_ERROR", directory.toAbsolutePath());
            }
        }

        return audioFiles;
    }

}
