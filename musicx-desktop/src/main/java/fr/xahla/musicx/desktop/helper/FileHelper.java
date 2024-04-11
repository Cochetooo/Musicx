package fr.xahla.musicx.desktop.helper;

import java.nio.file.Path;

/** <b>Utility class for files management.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public final class FileHelper {

    public static String getExtensionFromPath(final Path filepath) {
        final var filename = filepath.getFileName().toString();
        final var lastDotIndex = filename.lastIndexOf('.');

        return filename.substring(lastDotIndex + 1);
    }

}
