package fr.xahla.musicx.desktop.views;

import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.application.Platform;
import javafx.collections.ListChangeListener;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.Parent;
import javafx.scene.layout.VBox;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.library;

/**
 * Main view for the desktop application.
 * @author Cochetooo
 * @since 0.1.0
 */
public class Application implements Initializable {

    @FXML public VBox applicationBox;

    private Parent emptyLibraryComponent;
    private Parent contentComponent;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.emptyLibraryComponent = FxmlHelper.getComponent("content/emptyLibrary.fxml", resourceBundle);
        this.contentComponent = FxmlHelper.getComponent("content/contentLayout.fxml", resourceBundle);

        this.updateContent();

        library().onSongsChange(change -> Platform.runLater(this::updateContent));
    }

    private void updateContent() {
        if (library().isEmpty()) {
            this.applicationBox.getChildren().remove(this.contentComponent);

            if (!this.applicationBox.getChildren().contains(this.emptyLibraryComponent)) {
                this.applicationBox.getChildren().add(this.emptyLibraryComponent);
            }
        } else {
            this.applicationBox.getChildren().remove(this.emptyLibraryComponent);

            if (!this.applicationBox.getChildren().contains(this.contentComponent)) {
                this.applicationBox.getChildren().add(this.contentComponent);
            }
        }
    }
}
