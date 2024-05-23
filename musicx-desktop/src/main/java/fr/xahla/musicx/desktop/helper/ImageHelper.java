package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.DesktopApplication;
import javafx.scene.image.Image;
import javafx.scene.paint.Color;

import java.util.Objects;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Utility class for JavaFX Images.
 * @author Cochetooo
 * @since 0.2.4
 */
public final class ImageHelper {

    private final static String IMAGES_LOCATION = "assets/images/";

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

    /**
     * @since 0.3.3
     */
    public static Image loadImageFromResource(final String path) {
        final var imagePath = IMAGES_LOCATION + path;

        try {
            final var resource =
                Objects.requireNonNull(DesktopApplication.class.getResource(imagePath)).toExternalForm();

            return new Image(resource);
        } catch (final NullPointerException exception) {
            logger().error(exception, "IO_FILE_NOT_FOUND", imagePath);
            return null;
        } catch (final IllegalArgumentException exception) {
            logger().error(exception, "IO_URI_ERROR", imagePath);
            return null;
        }
    }

}
