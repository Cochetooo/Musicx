package fr.xahla.musicx.desktop.helper;

public enum FxmlComponent {

    EDIT_ALBUM("content/edit/albumEdit.fxml"),
    EDIT_SONG("content/edit/songEdit.fxml"),
    QUEUE_LIST("content/queueList.fxml"),

    MODAL_CONSOLE("console.fxml"),
    MODAL_IMPORT_FOLDERS("importFolders.fxml"),
    MODAL_SETTINGS("settings.fxml");

    private final String filepath;

    FxmlComponent(String filepath) {
        this.filepath = filepath;
    }

    public String getFilepath() {
        return filepath;
    }

}
