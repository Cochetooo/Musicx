package fr.xahla.musicx.desktop.helper;

/**
 * List of every fxml files components external to the core application.
 *
 * @author Cochetooo
 * @since 0.3.1
 */
public enum FxmlComponent {

    // Scenes
    SCENE_ENCYCLOPEDIA("content/encyclopedia/content.fxml"),
    SCENE_HISTORY("content/history/content.fxml"),
    SCENE_LOCAL_LIBRARY("content/localLibrary/content.fxml"),
    SCENE_PROFILE("content/profile/content.fxml"),
    SCENE_SETTINGS("content/settings/content.fxml"),
    SCENE_CONSOLE("content/console/content.fxml"),

    // Local Library Tabs
    LOCAL_LIBRARY_SONGS("content/localLibrary/songs.fxml"),

    // Right Nav
    EDIT_ALBUM("content/edit/albumEdit.fxml"),
    EDIT_SONG("content/edit/songEdit.fxml"),
    QUEUE_LIST("content/queueList.fxml"),

    // Modals
    MODAL_IMPORT_FOLDERS("importFolders.fxml"),
    MODAL_SETTINGS("content.fxml");

    private final String filepath;

    FxmlComponent(String filepath) {
        this.filepath = filepath;
    }

    public String getFilepath() {
        return filepath;
    }

}
