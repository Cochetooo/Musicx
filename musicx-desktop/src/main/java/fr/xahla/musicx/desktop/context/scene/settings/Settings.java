package fr.xahla.musicx.desktop.context.scene.settings;

import javafx.beans.property.BooleanProperty;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import org.json.JSONArray;

import java.util.ArrayList;

import static fr.xahla.musicx.desktop.context.DesktopContext.config;

/**
 * Global settings for desktop application.
 * @author Cochetooo
 * @since 0.2.2
 */
public class Settings {

    // Library
    private final ListProperty<String> scanLibraryAudioFormats;

    // Player
    private final BooleanProperty artworkShadow;
    private final BooleanProperty backgroundArtworkBind;
    private final BooleanProperty smoothPause;

    public Settings() {
        this.scanLibraryAudioFormats = new SimpleListProperty<>(FXCollections.observableList(
            new ArrayList<>(((JSONArray) config().getSetting("scanLibraryAudioFormats")).toList().stream()
                .map(Object::toString)
                .toList())
        ));

        this.artworkShadow = new SimpleBooleanProperty(
            (boolean) config().getSetting("artworkShadow")
        );

        this.backgroundArtworkBind = new SimpleBooleanProperty(
            (boolean) config().getSetting("backgroundArtworkBind")
        );

        this.smoothPause = new SimpleBooleanProperty(
            (boolean) config().getSetting("smoothPause")
        );
    }

    public ObservableList<String> getScanLibraryAudioFormats() {
        return scanLibraryAudioFormats.get();
    }

    public ListProperty<String> scanLibraryAudioFormatsProperty() {
        return scanLibraryAudioFormats;
    }

    public Settings setScanLibraryAudioFormats(final ObservableList<String> scanLibraryAudioFormats) {
        this.scanLibraryAudioFormats.set(scanLibraryAudioFormats);
        return this;
    }

    public boolean isArtworkShadow() {
        return artworkShadow.get();
    }

    public BooleanProperty artworkShadowProperty() {
        return artworkShadow;
    }

    public Settings setArtworkShadow(final boolean artworkShadow) {
        this.artworkShadow.set(artworkShadow);
        return this;
    }

    public boolean isBackgroundArtworkBind() {
        return backgroundArtworkBind.get();
    }

    public BooleanProperty backgroundArtworkBindProperty() {
        return backgroundArtworkBind;
    }

    public Settings setBackgroundArtworkBind(final boolean backgroundArtworkBind) {
        this.backgroundArtworkBind.set(backgroundArtworkBind);
        return this;
    }

    public boolean getSmoothPause() {
        return smoothPause.get();
    }

    public BooleanProperty smoothPauseProperty() {
        return smoothPause;
    }

    public Settings setSmoothPause(final boolean smoothPause) {
        this.smoothPause.set(smoothPause);
        return this;
    }
}
