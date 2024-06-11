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
    SCENE_LIBRARY("Pages/Library/Songs/SongList.fxml"),
    SCENE_SEARCH("Pages/Library/Search/Search.fxml"),
    SCENE_HISTORY("Pages/History/Summary.fxml"),
    SCENE_EDITOR("Pages/Editor/SongMatrix/SongMatrix.fxml"),
    SCENE_PROFILE("Pages/Profile/Profile.fxml"),
    SCENE_SETTINGS("Pages/Settings/Settings.fxml"),

    // Modals
    MODAL_LOCAL_FOLDERS("LocalFolders/LocalFolders.fxml"),
    MODAL_CONSOLE("Console/Console.fxml");

    private final String filepath;

    FxmlComponent(String filepath) {
        this.filepath = filepath;
    }

    public String getFilepath() {
        return filepath;
    }

}
