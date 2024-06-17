package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.context.scene.history.HistoryScene;
import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibraryScene;
import fr.xahla.musicx.desktop.context.scene.settings.Settings;
import fr.xahla.musicx.desktop.manager.ContentManager;
import fr.xahla.musicx.desktop.manager.TaskProgressManager;
import fr.xahla.musicx.desktop.model.entity.Album;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import javafx.beans.value.ChangeListener;

/**
 * Controller that handles global variables.
 * @author Cochetooo
 * @since 0.3.3
 */
public class SceneController {

    private final TaskProgressManager taskProgressManager;

    private final ContentManager sceneContent;
    private final ContentManager navContent;

    private final LocalLibraryScene localLibraryScene;
    private final HistoryScene historyScene;
    private final Settings settings;

    public SceneController() {
        taskProgressManager = new TaskProgressManager();

        sceneContent = new ContentManager();
        navContent = new ContentManager();

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

    public ContentManager getNavContent() { return navContent; }

    public TaskProgressManager getTaskProgress() {
        return taskProgressManager;
    }

}
