package fr.xahla.musicx.desktop.helper;

import javafx.scene.image.Image;
import javafx.scene.paint.Color;

/**
 * Utility class for JavaFX Images.
 * @author Cochetooo
 * @since 0.2.4
 */
public final class ImageHelper {

    /**
     * @since 0.2.4
     */
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
