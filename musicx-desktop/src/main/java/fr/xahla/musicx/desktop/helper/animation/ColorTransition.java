package fr.xahla.musicx.desktop.helper.animation;

import javafx.animation.KeyFrame;
import javafx.animation.Timeline;
import javafx.scene.paint.Color;
import javafx.util.Duration;

import java.util.concurrent.atomic.AtomicInteger;

/**
 * Animation for colors.
 * @author Cochetooo
 * @since 0.3.3
 */
public record ColorTransition(
    Duration duration,
    Color startColor,
    Color endColor,
    int timeStep,
    FadeAnimationSet set
) {
    public void play() {
        final var numFrames = (int) (duration.toMillis() / timeStep);
        final var i = new AtomicInteger();

        final var timeline = new Timeline(
            new KeyFrame(Duration.millis(timeStep), event -> {
                final var newColor = startColor.interpolate(endColor, (1.0 / numFrames) * i.get());

                set.set(newColor);
                i.getAndIncrement();
            })
        );

        timeline.setCycleCount(numFrames + 1);
        timeline.play();
    }

    public interface FadeAnimationSet {
        void set(final Color color);
    }
}
