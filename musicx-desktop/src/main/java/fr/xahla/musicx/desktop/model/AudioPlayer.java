package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import javafx.beans.property.DoubleProperty;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.beans.property.SimpleObjectProperty;

/**
 * Handles audio player data.
 * @author Cochetooo
 */
public class AudioPlayer {

    private final DoubleProperty volume;
    private final ObjectProperty<Song> song;
    private final ObjectProperty<ShuffleMode> shuffleMode;
    private final ObjectProperty<RepeatMode> repeatMode;

    public AudioPlayer() {
        this.volume = new SimpleDoubleProperty(0.5);
        this.song = new SimpleObjectProperty<>(null);
        this.shuffleMode = new SimpleObjectProperty<>(ShuffleMode.LINEAR);
        this.repeatMode = new SimpleObjectProperty<>(RepeatMode.NO_REPEAT);
    }

    public double getVolume() {
        return volume.get();
    }

    public DoubleProperty volumeProperty() {
        return volume;
    }

    public AudioPlayer setVolume(double volume) {
        this.volume.set(volume);
        return this;
    }

    public Song getSong() {
        return song.get();
    }

    public ObjectProperty<Song> songProperty() {
        return song;
    }

    public AudioPlayer setSong(Song song) {
        this.song.set(song);
        return this;
    }

    public ShuffleMode getShuffleMode() {
        return shuffleMode.get();
    }

    public ObjectProperty<ShuffleMode> shuffleModeProperty() {
        return shuffleMode;
    }

    public AudioPlayer setShuffleMode(ShuffleMode shuffleMode) {
        this.shuffleMode.set(shuffleMode);
        return this;
    }

    public RepeatMode getRepeatMode() {
        return repeatMode.get();
    }

    public ObjectProperty<RepeatMode> repeatModeProperty() {
        return repeatMode;
    }

    public AudioPlayer setRepeatMode(RepeatMode repeatMode) {
        this.repeatMode.set(repeatMode);
        return this;
    }

}
