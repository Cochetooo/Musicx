package fr.xahla.musicx.desktop.context;

import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

/**
 * Controller that handles global variables.
 * @author Cochetooo
 * @since 0.3.3
 */
public class Controller {

    private final StringProperty searchText;

    public Controller() {
        searchText = new SimpleStringProperty();
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
