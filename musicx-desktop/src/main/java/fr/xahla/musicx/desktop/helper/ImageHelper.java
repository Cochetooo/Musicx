package fr.xahla.musicx.desktop.helper;

import javafx.scene.image.Image;
import javafx.scene.paint.Color;

/** <b>Utility class for JavaFX Images.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class ImageHelper {

    public static Color calculateAverageColor(final Image image) {
        final int width = (int) image.getWidth();
        final int height = (int) image.getHeight();

        var totalRed = 0.0;
        var totalGreen = 0.0;
        var totalBlue = 0.0;

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                final var color = image.getPixelReader().getColor(x, y);
                totalRed += color.getRed();
                totalGreen += color.getGreen();
                totalBlue += color.getBlue();
            }
        }

        final var numPixels = width * height;
        final var avgRed = totalRed / numPixels;
        final var avgGreen = totalGreen / numPixels;
        final var avgBlue = totalBlue / numPixels;

        return Color.color(avgRed, avgGreen, avgBlue);
    }

}
