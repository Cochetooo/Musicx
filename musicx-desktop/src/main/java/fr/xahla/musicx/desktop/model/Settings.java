package fr.xahla.musicx.desktop.model;

import javafx.beans.property.BooleanProperty;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;

public class Settings {

    // Library
    private final ListProperty<String> scanLibraryAudioFormats;

    // Player
    private final BooleanProperty smoothFadeStop;

    public Settings() {
        this.scanLibraryAudioFormats = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.smoothFadeStop = new SimpleBooleanProperty();
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
