package fr.xahla.musicx.domain.helper;

import java.nio.file.Path;

/**
 * Utility class for file management.
 * @author Cochetooo
 * @since 0.1.0
 */
public final class FileHelper {

    /**
     * @since 0.1.0
     */
    public static String file_get_extension(final Path filepath) {
        final var filename = filepath.getFileName().toString();
        final var lastDotIndex = filename.lastIndexOf('.');

        return filename.substring(lastDotIndex + 1);
    }

}
