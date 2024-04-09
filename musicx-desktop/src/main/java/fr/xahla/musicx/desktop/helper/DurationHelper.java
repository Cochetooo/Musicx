package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.logging.ErrorMessage;

import java.time.Duration;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public final class DurationHelper {

    public static boolean benchmark = true;

    public static String getDurationFormatted(final Duration duration) {
        var hours = duration.toHours();
        var minutes = duration.toMinutesPart();
        var seconds = duration.toSecondsPart();

        if (hours > 0) {
            return String.format("%d:%02d:%02d", hours, minutes, seconds);
        } else {
            return String.format("%02d:%02d", minutes, seconds);
        }
    }

    public static String getTimeString(double millis) {
        millis /= 1000;
        String s = formatTime(millis % 60);
        millis /= 60;
        String m = formatTime(millis % 60);
        millis /= 60;
        String h = formatTime(millis % 24);
        return h + ":" + m + ":" + s;
    }

    public static String formatTime(final double time) {
        int t = (int)time;
        if (t > 9) { return String.valueOf(t); }
        return "0" + t;
    }

    /**
     * Display the time elapsed between the start time and now in milliseconds.
     * @param context The context of this benchmark
     * @param startTime The start time in millis.
     */
    public static void benchmarkFrom(final String context, final long startTime) {
        if (!benchmark) {
            return;
        }

        logger().fine(ErrorMessage.BENCHMARK.getMsg(context, System.currentTimeMillis() - startTime));
    }

}
