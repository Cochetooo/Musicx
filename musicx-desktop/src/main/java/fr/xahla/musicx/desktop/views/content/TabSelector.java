package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Tab;
import javafx.scene.control.TabPane;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.3.3
 */
public class TabSelector implements Initializable {

    @FXML private TabPane pageTab;

    @FXML private Tab localLibraryTab;
    @FXML private Tab encyclopediaTab;
    @FXML private Tab historyTab;
    @FXML private Tab editorTab;
    @FXML private Tab profileTab;
    @FXML private Tab settingsTab;
    @FXML private Tab consoleTab;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        pageTab.getSelectionModel().selectedItemProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue == localLibraryTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_LOCAL_LIBRARY,
                    resourceBundle
                );
            } else if (newValue == encyclopediaTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_ENCYCLOPEDIA,
                    resourceBundle
                );
            } else if (newValue == historyTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_HISTORY,
                    resourceBundle
                );
            } else if (newValue == editorTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_EDITOR,
                    resourceBundle
                );
            } else if (newValue == profileTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_PROFILE,
                    resourceBundle
                );
            } else if (newValue == settingsTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_SETTINGS,
                    resourceBundle
                );
            } else if (newValue == consoleTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_CONSOLE,
                    resourceBundle
                );
            }
        });
    }

}
