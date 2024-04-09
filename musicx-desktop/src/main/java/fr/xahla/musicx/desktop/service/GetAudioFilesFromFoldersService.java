package fr.xahla.musicx.desktop.service;

import fr.xahla.musicx.desktop.helper.ArrayHelper;
import fr.xahla.musicx.desktop.helper.FileHelper;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public final class GetAudioFilesFromFoldersService {

    public static List<Path> execute(
        final List<String> paths,
        final String[] acceptedFormats
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
                logger().severe(exception.getLocalizedMessage());
            }
        }

        return audioFiles;
    }

}
