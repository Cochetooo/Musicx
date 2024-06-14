package fr.xahla.musicx.desktop.views.pages.editor;

import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.views.components.ContentSwitchTab;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TabPane;
import javafx.scene.layout.StackPane;

import java.net.URL;
import java.util.List;
import java.util.ResourceBundle;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class EditorLayout implements Initializable {

    @FXML private ContentSwitchTab contentSwitchTab;

    @FXML private StackPane pageContainer;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        contentSwitchTab.setTabs(List.of(
            // Artists Tab
            new ContentSwitchTab.TabInfo(
                "Artist",
                FxmlComponent.EDITOR_LIST_ARTIST,
                "editor.layout.tabArtist",
                null,
                0.0,
                null
            ),

            // Albums Tab
            new ContentSwitchTab.TabInfo(
                "Album",
                FxmlComponent.EDITOR_LIST_ARTIST,
                "editor.layout.tabAlbum",
                null,
                0.0,
                null
            )
        ));

        contentSwitchTab.setPageContainer(pageContainer);
        contentSwitchTab.setClosingPolicy(TabPane.TabClosingPolicy.UNAVAILABLE);
        contentSwitchTab.setDragPolicy(TabPane.TabDragPolicy.REORDER);
    }

}
