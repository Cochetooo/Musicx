package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the main center content with the track list and the search bar.
 * @author Cochetooo
 * @since 0.2.0
 */
public class Content implements Initializable {

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
    }

    @FXML public void actionSettings() {
        FxmlHelper.showModal(FxmlComponent.MODAL_SETTINGS, this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void actionQueueList() {
        final var rightNavContent = scene().getRightNavContent();

        if (null != rightNavContent.get()) {
            rightNavContent.close();
        } else {
            rightNavContent.switchContent(FxmlComponent.QUEUE_LIST, this.resourceBundle);
        }
    }
}
