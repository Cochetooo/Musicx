package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.views.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.ButtonType;
import javafx.stage.Stage;

import java.io.IOException;
import java.util.Objects;
import java.util.Optional;

import static fr.xahla.musicx.desktop.context.DesktopContext.globalResourceBundle;
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
    public static Parent getComponent(final String fxmlSource) {
        try {
             return FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource(fxmlSource)),
                globalResourceBundle()
            );
        } catch (final IOException exception) {
            logger().error(exception, "FXML_COMPONENT_LOAD_ERROR", fxmlSource);

            return null;
        }
    }

    /**
     * @since 0.2.0
     */
    public static void showModal(final FxmlComponent fxmlSource, final String title) {
        final var stage = new Stage();

        try {
            final Parent fxmlComponent = FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource(fxmlSource.getFilepath())),
                globalResourceBundle()
            );

            final var scene = new Scene(fxmlComponent);
            stage.setScene(scene);
            stage.setTitle(globalResourceBundle().getString(title));
            stage.show();
        } catch (final IOException exception) {
            logger().error(exception, "FXML_MODAL_LOAD_ERROR", fxmlSource);
        }
    }

    /**
     * @since 0.5.0
     */
    public static Optional<ButtonType> showInformationAlert(final String title, final String message) {
        final var alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(message);
        return alert.showAndWait();
    }

    /**
     * @since 0.5.0
     */
    public static Optional<ButtonType> showConfirmAlert(final String title, final String message) {
        final var alert = new Alert(Alert.AlertType.CONFIRMATION);
        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(message);
        return alert.showAndWait();
    }

}
