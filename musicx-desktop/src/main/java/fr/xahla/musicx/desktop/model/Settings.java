package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.domain.application.SettingsInterface;
import javafx.beans.property.BooleanProperty;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;

public class Settings implements SettingsInterface {

    // Library
    private final ListProperty<String> scanLibraryAudioFormats;

    // Player
    private final BooleanProperty artworkShadow;
    private final BooleanProperty backgroundArtworkBind;
    private final BooleanProperty smoothFadeStop;

    public Settings() {
        this.scanLibraryAudioFormats = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

        this.artworkShadow = new SimpleBooleanProperty(true);
        this.backgroundArtworkBind = new SimpleBooleanProperty(true);
        this.smoothFadeStop = new SimpleBooleanProperty(true);
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

    public boolean isSmoothFadeStop() {
        return smoothFadeStop.get();
    }

    public BooleanProperty smoothFadeStopProperty() {
        return smoothFadeStop;
    }

    public Settings setSmoothFadeStop(final boolean smoothFadeStop) {
        this.smoothFadeStop.set(smoothFadeStop);
        return this;
    }
}
