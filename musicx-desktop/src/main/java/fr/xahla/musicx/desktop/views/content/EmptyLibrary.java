package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

/**
 * View for empty library use case.
 * @author Cochetooo
 * @since 0.2.0
 */
public class EmptyLibrary implements Initializable {

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
    }

    @FXML public void importFolders() {
        FxmlHelper.showModal(FxmlComponent.MODAL_IMPORT_FOLDERS, this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }
}
