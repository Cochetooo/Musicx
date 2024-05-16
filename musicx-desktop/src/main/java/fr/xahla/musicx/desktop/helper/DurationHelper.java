package fr.xahla.musicx.desktop.helper;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.util.Duration;

/**
 * Duration utility class
 * @author Cochetooo
 * @since 0.1.0
 */
public final class DurationHelper {

    /**
     * @since 0.1.0
     */
    public static String getTimeString(final Duration duration) {
        return fr.xahla.musicx.domain.helper.DurationHelper.getTimeString(java.time.Duration.ofMillis((long) duration.toMillis()));
    }

}
