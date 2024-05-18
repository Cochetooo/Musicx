package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.views.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.io.IOException;
import java.util.Objects;
import java.util.ResourceBundle;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Utility class for FXML components.
 * @author Cochetooo
 * @since 0.2.0
 */
public final class FxmlHelper {

    /**
     * @return A parent component, otherwise <b>null</b> if the component could not be loaded.
     * @since 0.2.0
     */
    public static Parent getComponent(final String fxmlSource, final ResourceBundle resourceBundle) {
        try {
             return FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource(fxmlSource)),
                resourceBundle
            );
        } catch (final IOException exception) {
            logger().error(exception, "FXML_COMPONENT_LOAD_ERROR", fxmlSource);

            return null;
        }
    }

    /**
     * @since 0.2.0
     */
    public static void showModal(final FxmlComponent fxmlSource, final ResourceBundle resourceBundle, final String title) {
        final var stage = new Stage();

        try {
            final Parent fxmlComponent = FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource("modal/" + fxmlSource.getFilepath())),
                resourceBundle
            );

            final var scene = new Scene(fxmlComponent);
            stage.setScene(scene);
            stage.setTitle(title);
            stage.show();
        } catch (final IOException exception) {
            logger().error(exception, "FXML_MODAL_LOAD_ERROR", fxmlSource);
        }
    }

}
