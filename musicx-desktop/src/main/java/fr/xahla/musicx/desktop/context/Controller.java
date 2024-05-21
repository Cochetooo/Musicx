package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.context.scene.history.HistoryScene;
import fr.xahla.musicx.desktop.context.scene.localLibrary.LocalLibraryScene;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

/**
 * Controller that handles global variables.
 * @author Cochetooo
 * @since 0.3.3
 */
public class Controller {

    private final StringProperty searchText;

    private final LocalLibraryScene localLibraryScene;
    private final HistoryScene historyScene;

    public Controller() {
        searchText = new SimpleStringProperty();

        localLibraryScene = new LocalLibraryScene();
        historyScene = new HistoryScene();
    }

    public LocalLibraryScene getLocalLibraryScene() {
        return localLibraryScene;
    }

    public HistoryScene getHistoryScene() {
        return historyScene;
    }

    public String getSearchText() {
        return searchText.get();
    }

    public Controller setSearchText(String searchText) {
        this.searchText.set(searchText);
        return this;
    }

    public StringProperty searchTextProperty() {
        return searchText;
    }

}
