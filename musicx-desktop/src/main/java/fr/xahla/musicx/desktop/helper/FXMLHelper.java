package fr.xahla.musicx.desktop.helper;


import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.views.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;

import java.io.IOException;
import java.util.Objects;
import java.util.ResourceBundle;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class FXMLHelper {

    public static Parent getComponent(final String fxmlSource, final ResourceBundle resourceBundle) {
        try {
             return FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource(fxmlSource)),
                resourceBundle
            );
        } catch (IOException e) {
            logger().severe(ErrorMessage.LOAD_FXML_COMPONENT_ERROR.getMsg(fxmlSource));
            logger().severe(e.getLocalizedMessage());
            e.printStackTrace();
        }

        return null;
    }

    public static void showModal(final String fxmlSource, final ResourceBundle resourceBundle, final String title) {
        final var stage = new Stage();

        try {
            final Parent fxmlComponent = FXMLLoader.load(
                Objects.requireNonNull(Application.class.getResource("modal/" + fxmlSource)),
                resourceBundle
            );

            final var scene = new Scene(fxmlComponent);
            stage.setScene(scene);
            stage.setTitle(title);
            stage.show();
        } catch (IOException e) {
            logger().severe(ErrorMessage.LOAD_FXML_MODAL_ERROR.getMsg(fxmlSource, title));
            e.printStackTrace();
        }
    }

}
