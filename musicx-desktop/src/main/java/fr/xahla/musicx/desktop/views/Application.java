package fr.xahla.musicx.desktop.views;

import fr.xahla.musicx.desktop.helper.FXMLHelper;
import javafx.application.Platform;
import javafx.collections.ListChangeListener;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.Parent;
import javafx.scene.layout.VBox;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.library;

/** <b>Main view for the desktop application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Application implements Initializable {

    @FXML public VBox applicationBox;

    private Parent emptyLibraryComponent;
    private Parent contentComponent;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.emptyLibraryComponent = FXMLHelper.getComponent("content/emptyLibrary.fxml", resourceBundle);
        this.contentComponent = FXMLHelper.getComponent("content/contentLayout.fxml", resourceBundle);

        this.updateContent();

        library().getFolderPaths().addListener((ListChangeListener<String>) change
            -> Platform.runLater(this::updateContent));
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
