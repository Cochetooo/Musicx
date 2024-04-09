package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.FXMLHelper;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

public class EmptyLibrary implements Initializable {

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
    }

    @FXML public void importFolders() {
        FXMLHelper.showModal("importFolders.fxml", this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }
}
