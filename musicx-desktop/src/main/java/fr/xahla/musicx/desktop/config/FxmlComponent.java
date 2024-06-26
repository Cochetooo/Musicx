package fr.xahla.musicx.desktop.config;

/**
 * List of every fxml files components external to the core application.
 *
 * @author Cochetooo
 * @since 0.3.1
 */
public enum FxmlComponent {

    // Components
    COMPONENT_TOAST("Components/Toast.fxml"),

    // Edit
    EDIT_GENRE("Pages/Editor/Genre/InlineEditGenre.fxml"),

    // Editor
    EDITOR_LIST_ARTIST("Pages/Editor/Artist/ListArtist.fxml"),

    // Scenes
    SCENE_LIBRARY("Pages/Library/Songs/SongList.fxml"),
    SCENE_LIBRARY_ALBUM("Pages/Library/Album/ShowAlbum.fxml"),
    SCENE_SEARCH("Pages/Library/Search/Search.fxml"),
    SCENE_NOW_PLAYING("Pages/Visualizer/NowPlaying.fxml"),
    SCENE_HISTORY("Pages/History/Summary.fxml"),
    SCENE_EDITOR("Pages/Editor/EditorLayout.fxml"),
    SCENE_PROFILE("Pages/Profile/Profile.fxml"),
    SCENE_SETTINGS("Pages/Settings/Settings.fxml"),

    // Modals
    MODAL_LOCAL_FOLDERS("Modals/LocalFolders/LocalFolders.fxml"),
    MODAL_CONSOLE("Modals/Console/Console.fxml"),

    // Navigation
    NAV_QUEUE_LIST("Navigation/QueueList.fxml");

    private final String filepath;

    FxmlComponent(String filepath) {
        this.filepath = filepath;
    }

    public String getFilepath() {
        return filepath;
    }

}
