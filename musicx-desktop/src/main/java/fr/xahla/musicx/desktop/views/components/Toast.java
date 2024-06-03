package fr.xahla.musicx.desktop.views.components;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.animation.KeyFrame;
import javafx.animation.KeyValue;
import javafx.animation.Timeline;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.paint.Color;
import javafx.stage.Stage;
import javafx.stage.StageStyle;
import javafx.util.Duration;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Toast component for FXML
 * @author Cochetooo
 * @since 0.4.1
 */
public final class Toast {

    private static final Parent toastFxml;
    private static final Scene toastScene;

    static {
        toastFxml = FxmlHelper.getComponent(FxmlComponent.COMPONENT_TOAST.getFilepath(), null);

        if (null == toastFxml) {
            logger().severe("FXML_COMPONENT_TOAST_ERROR");
            throw new RuntimeException();
        }

        toastFxml.setOpacity(0);

        toastScene = new Scene(toastFxml);
        toastScene.setFill(Color.TRANSPARENT);
    }

    public static void showToast(final String message, final int delay, final int fadeInDelay, final int fadeOutDelay) {
        final var toastStage = new Stage();
        toastStage.initOwner(toastFxml.getScene().getWindow());
        toastStage.setResizable(false);
        toastStage.initStyle(StageStyle.TRANSPARENT);
        toastStage.setScene(toastScene);
        toastStage.show();

        final var fadeInTimeline = new Timeline();
        final var fadeInKey1 = new KeyFrame(Duration.millis(fadeInDelay), new KeyValue(toastStage.getScene().getRoot().opacityProperty(), 1));
        fadeInTimeline.getKeyFrames().add(fadeInKey1);
        fadeInTimeline.setOnFinished((ae) -> {
            new Thread(() -> {
                try {
                    Thread.sleep(delay);
                } catch (final InterruptedException exception) {
                    logger().error(exception, "THREAD_INTERRUPTED", "Toast Animation");
                }

                final var fadeOutTimeline = new Timeline();
                final var fadeOutKey1 = new KeyFrame(Duration.millis(fadeOutDelay), new KeyValue(toastStage.getScene().getRoot().opacityProperty(), 0));
                fadeOutTimeline.getKeyFrames().add(fadeOutKey1);
                fadeOutTimeline.setOnFinished((aeb) -> toastStage.close());
                fadeOutTimeline.play();
            }).start();
        });

        fadeInTimeline.play();
    }

}
