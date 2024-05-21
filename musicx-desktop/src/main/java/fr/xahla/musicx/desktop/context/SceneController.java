package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.context.scene.history.HistoryScene;
import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibraryScene;
import fr.xahla.musicx.desktop.context.scene.settings.Settings;
import fr.xahla.musicx.desktop.manager.ContentManager;
import fr.xahla.musicx.desktop.manager.TaskProgressManager;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import javafx.beans.value.ChangeListener;

/**
 * Controller that handles global variables.
 * @author Cochetooo
 * @since 0.3.3
 */
public class SceneController {

    private final StringProperty searchText;
    private final TaskProgressManager taskProgressManager;

    private final ContentManager sceneContent;
    private final ContentManager rightNavContent;

    private final LocalLibraryScene localLibraryScene;
    private final HistoryScene historyScene;
    private final Settings settings;

    public SceneController() {
        searchText = new SimpleStringProperty();
        taskProgressManager = new TaskProgressManager();

        sceneContent = new ContentManager();
        rightNavContent = new ContentManager();

        localLibraryScene = new LocalLibraryScene();
        historyScene = new HistoryScene();

        settings = new Settings();
    }

    // --- Getters / Setters ---

    public LocalLibraryScene getLocalLibraryScene() {
        return localLibraryScene;
    }

    public HistoryScene getHistoryScene() {
        return historyScene;
    }

    public Settings getSettings() {
        return settings;
    }

    public ContentManager getSceneContent() {
        return sceneContent;
    }

    public ContentManager getRightNavContent() {
        return rightNavContent;
    }

    public TaskProgressManager getTaskProgress() {
        return taskProgressManager;
    }

    public String getSearchText() {
        return searchText.get();
    }

    public SceneController setSearchText(String searchText) {
        this.searchText.set(searchText);
        return this;
    }

    // --- Listeners ---

    public void bindSearchText(final StringProperty searchText) {
        this.searchText.bind(searchText);
    }

    public void onSearchTextChange(final ChangeListener<String> listener) {
        searchText.removeListener(listener);
        searchText.addListener(listener);
    }

}
