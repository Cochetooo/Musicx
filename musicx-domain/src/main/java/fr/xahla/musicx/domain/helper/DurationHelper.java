package fr.xahla.musicx.domain.helper;

import java.time.DateTimeException;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Utility class for durations.
 * @author Cochetooo
 * @since 0.3.2
 */
public final class DurationHelper {

    public static final LocalDate FIRST_DAY = LocalDate.of(1, 1, 1);

    public static String getDateTimeString(final LocalDateTime date) {
        final var formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

        try {
            return date.format(formatter);
        } catch (final DateTimeException exception) {
            logger().warn("DATE_FORMAT_INCORRECT", date.toString());
            return "0000-00-00 00:00:00";
        }
    }

    /**
     * @since 0.3.2
     */
    public static String getTimeString(final Duration duration) {
        var hours = duration.toHours();
        var minutes = duration.toMinutesPart();
        var seconds = duration.toSecondsPart();

        if (hours > 0) {
            return String.format("%d:%02d:%02d", hours, minutes, seconds);
        } else {
            return String.format("%02d:%02d", minutes, seconds);
        }
    }

    /**
     * @since 0.3.2
     */
    public static String getTimeString(double millis) {
        millis /= 1000;
        String s = formatTime(millis % 60);
        millis /= 60;
        String m = formatTime(millis % 60);
        millis /= 60;
        String h = formatTime(millis % 24);
        return h + ":" + m + ":" + s;
    }

    /**
     * @since 0.3.2
     */
    public static String formatTime(final double time) {
        int t = (int)time;
        if (t > 9) { return String.valueOf(t); }
        return "0" + t;
    }

}
