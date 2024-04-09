package fr.xahla.musicx.desktop.helper;

import java.nio.file.Path;

public final class FileHelper {

    public static String getExtensionFromPath(final Path filepath) {
        final var filename = filepath.getFileName().toString();
        final var lastDotIndex = filename.lastIndexOf('.');

        return filename.substring(lastDotIndex + 1);
    }

}
