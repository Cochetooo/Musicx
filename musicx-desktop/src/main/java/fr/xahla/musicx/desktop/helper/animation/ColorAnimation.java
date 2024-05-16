package fr.xahla.musicx.desktop.helper.animation;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.util.Duration;

/**
 * Create a color animation fading.
 *
 * @author Cochetooo
 * @since 0.3.2
 */
public record ColorAnimation(
    Duration duration,
    double startValue,
    double endValue,
    int timeStep,
    FadeAnimationGet get,
    FadeAnimationSet set,
    FadeAnimationCallback callback
) {

    /**
     * @since 0.3.2
     */
    public void fade() {
        final var numFrames = (int) (duration.toMillis() / timeStep);
        final var valueStep = (startValue - endValue) / numFrames;

        final var timeline = new Timeline(
            new KeyFrame(Duration.millis(timeStep), event -> {
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

    // Interfaces

    /**
     * @since 0.2.4
     */
    public interface FadeAnimationSet {
        void set(double val);
    }

    /**
     * @since 0.2.4
     */
    public interface FadeAnimationGet {
        double get();
    }

    /**
     * @since 0.2.4
     */
    public interface FadeAnimationCallback {
        void done();
    }

}
