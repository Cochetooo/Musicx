package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.FXMLHelper;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

/** <b>View for empty library use case.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class EmptyLibrary implements Initializable {

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
    }

    @FXML public void importFolders() {
        FXMLHelper.showModal("importFolders.fxml", this.resourceBundle, resourceBundle.getString("importFolders.title"));
    }
}
