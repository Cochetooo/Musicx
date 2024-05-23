package fr.xahla.musicx.desktop.context.scene.localLibrary;

import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.model.enums.LocalSongGroup;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.value.ChangeListener;
import javafx.collections.transformation.FilteredList;

import java.util.function.Predicate;

/**
 * Defines all global variables for the local library scene.
 * @author Cochetooo
 * @since 0.4.0
 */
public final class LocalLibraryScene {

    private final LocalLibrary localLibrary;
    private final FilteredList<Song> trackList;

    private final ObjectProperty<LocalSongGroup> currentListGroup;

    public LocalLibraryScene() {
        this.localLibrary = new LocalLibrary();

        this.trackList = new FilteredList<>(localLibrary.getLocalSongs());

        this.currentListGroup = new SimpleObjectProperty<>(LocalSongGroup.ARTISTS);
    }

    // --- Getters / Setters ---

    public LocalLibrary getLibrary() {
        return localLibrary;
    }

    public FilteredList<Song> getTrackList() {
        return trackList;
    }

    public void setTrackListFilters(final Predicate<Song> filter) {
        trackList.setPredicate(filter);
    }

    public LocalSongGroup getCurrentListGroup() {
        return currentListGroup.get();
    }

    public void setCurrentListGroup(final LocalSongGroup currentListGroup) {
        this.currentListGroup.set(currentListGroup);
    }

    // --- Listeners ---

    public void onListGroupChange(final ChangeListener<LocalSongGroup> listener) {
        currentListGroup.addListener(listener);
    }

}
