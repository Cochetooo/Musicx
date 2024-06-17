package fr.xahla.musicx.desktop.context.scene.localLibrary;

import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
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

    private final ObjectProperty<Album> currentAlbum;

    public LocalLibraryScene() {
        this.localLibrary = new LocalLibrary();

        this.trackList = new FilteredList<>(localLibrary.getLocalSongs());
        this.currentAlbum = new SimpleObjectProperty<>();
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

    public Album getCurrentAlbum() {
        return currentAlbum.get();
    }

    public ObjectProperty<Album> currentAlbumProperty() {
        return currentAlbum;
    }

    public LocalLibraryScene setCurrentAlbum(final Album album) {
        currentAlbum.set(album);
        return this;
    }

    // --- Listeners ---

    public void resetFilters() {
        trackList.setPredicate(null);
    }
}
