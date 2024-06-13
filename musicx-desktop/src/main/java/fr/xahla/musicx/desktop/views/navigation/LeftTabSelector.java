package fr.xahla.musicx.desktop.views.navigation;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Tab;
import javafx.scene.control.TabPane;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * @author Cochetooo
 * @since 0.3.3
 */
public class LeftTabSelector implements Initializable {

    @FXML private TabPane pageTab;

    @FXML private Tab libraryTab;
    @FXML private Tab searchTab;
    @FXML private Tab nowPlayingTab;
    @FXML private Tab historyTab;
    @FXML private Tab editorTab;
    @FXML private Tab profileTab;
    @FXML private Tab settingsTab;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        audioPlayer().onSongChange(listener -> nowPlayingTab.setDisable(false));

        pageTab.getSelectionModel().selectedItemProperty().addListener((observable, oldValue, newValue) -> {
            if (newValue == libraryTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_LIBRARY,
                    resourceBundle
                );
            } else if (newValue == searchTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_SEARCH,
                    resourceBundle
                );
            } else if (newValue == nowPlayingTab) {
                scene().getSceneContent().switchContent(
                    FxmlComponent.SCENE_NOW_PLAYING,
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
            }
        });
    }

}
