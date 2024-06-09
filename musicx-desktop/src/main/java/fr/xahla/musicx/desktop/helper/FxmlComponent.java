package fr.xahla.musicx.desktop.helper;

/**
 * List of every fxml files components external to the core application.
 *
 * @author Cochetooo
 * @since 0.3.1
 */
public enum FxmlComponent {

    // Components
    COMPONENT_TOAST("Components/Toast.fxml"),

    // Scenes
    SCENE_LOCAL_LIBRARY("content/localLibrary/Settings.fxml"),
    SCENE_ENCYCLOPEDIA("content/encyclopedia/Settings.fxml"),
    SCENE_HISTORY("content/history/Settings.fxml"),
    SCENE_EDITOR("content/editor/Settings.fxml"),
    SCENE_PROFILE("content/profile/Settings.fxml"),
    SCENE_SETTINGS("content/settings/Settings.fxml"),
    SCENE_CONSOLE("content/console/Settings.fxml"),

    // Local Library Tabs
    LOCAL_LIBRARY_ALBUMS("content/localLibrary/albums.fxml"),
    LOCAL_LIBRARY_SONGS("content/localLibrary/songs.fxml"),

    // Right Nav
    EDIT_ALBUM("content/edit/albumEdit.fxml"),
    EDIT_GENRE("content/edit/genreEdit.fxml"),
    EDIT_SONG("content/edit/songEdit.fxml"),
    QUEUE_LIST("content/queueList.fxml"),

    // Modals
    MODAL_IMPORT_FOLDERS("importFolders.fxml"),
    MODAL_SETTINGS("Settings.fxml");

    private final String filepath;

    FxmlComponent(String filepath) {
        this.filepath = filepath;
    }

    public String getFilepath() {
        return filepath;
    }

}
