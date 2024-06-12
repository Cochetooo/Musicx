package fr.xahla.musicx.desktop.helper;

import javafx.animation.KeyFrame;
import javafx.animation.KeyValue;
import javafx.animation.Timeline;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.scene.control.ScrollBar;
import javafx.scene.control.TableView;
import javafx.util.Duration;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public final class TableHelper {

    public static ScrollBar getVerticalScrollBar(final TableView<?> tableView) {
        for (final var node : tableView.lookupAll(".scroll-bar")) {
            if (node.getStyleClass().contains("vertical-scroll-bar")) {
                return (ScrollBar) node;
            }
        }

        return null;
    }

    public static void smoothScrollTo(final TableView<?> tableView, final double position) {
        final var scrollBar = getVerticalScrollBar(tableView);
        if (null != scrollBar) {
            final var scrollPosition = new SimpleDoubleProperty(scrollBar.getValue());

            final var timeline = new Timeline(
                new KeyFrame(Duration.ZERO, e -> scrollBar.setValue(scrollPosition.get())),
                new KeyFrame(Duration.seconds(1), new KeyValue(scrollPosition, position))
            );

            timeline.setCycleCount(1);
            timeline.play();
        }
    }

}
