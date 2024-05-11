package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.logging.ErrorMessage;
import javafx.animation.KeyFrame;
import javafx.animation.Timeline;

import javafx.util.Duration;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/** <b>Utility class for durations.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 * @see javafx.util.Duration
 */
public final class DurationHelper {

    public static boolean benchmark = true;

    public static String getTimeString(final java.time.Duration duration) {
        var hours = duration.toHours();
        var minutes = duration.toMinutesPart();
        var seconds = duration.toSecondsPart();

        if (hours > 0) {
            return String.format("%d:%02d:%02d", hours, minutes, seconds);
        } else {
            return String.format("%02d:%02d", minutes, seconds);
        }
    }

    public static String getTimeString(final Duration duration) {
        return getTimeString(java.time.Duration.ofMillis((long) duration.toMillis()));
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

    public interface FadeAnimationSet {
        void set(double val);
    }

    public interface FadeAnimationGet {
        double get();
    }

    public interface FadeAnimationCallback {
        void done();
    }

    public static void fade(
        final javafx.util.Duration duration,
        final double startValue,
        final double endValue,
        final int timeStep,
        final FadeAnimationSet set,
        final FadeAnimationGet get,
        final FadeAnimationCallback callback
    ) {
        final var numFrames = (int) (duration.toMillis() / timeStep);
        final var valueStep = (startValue - endValue) / numFrames;

        final var timeline = new Timeline(
            new KeyFrame(javafx.util.Duration.millis(timeStep), event -> {
                final var newValue = get.get() - valueStep;
                if (newValue <= endValue) {
                    set.set(endValue);
                    callback.done();
                } else {
                    set.set(newValue);
                }
            })
        );

        timeline.setCycleCount(numFrames + 1);
        timeline.play();
    }

}
