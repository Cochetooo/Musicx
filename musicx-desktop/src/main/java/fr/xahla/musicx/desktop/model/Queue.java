package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.desktop.listener.ValueListener;
import fr.xahla.musicx.desktop.logging.LogMessageFX;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.log;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Handles queue data.
 * @author Cochetooo
 * @since 0.2.2
 */
public class Queue {

    private final ListProperty<Song> songs;
    private final LongProperty duration;
    private final IntegerProperty songCount;
    private final IntegerProperty position;

    private final List<ValueListener<Integer>> positionListeners;

    public Queue() {
        this.songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.duration = new SimpleLongProperty();
        this.songCount = new SimpleIntegerProperty();
        this.position = new SimpleIntegerProperty();

        this.positionListeners = new ArrayList<>();
    }

    public ObservableList<Song> getSongs() {
        return songs.get();
    }

    public ListProperty<Song> songsProperty() {
        return songs;
    }

    public Queue setSongs(final ObservableList<Song> songs) {
        this.songs.set(songs);
        return this;
    }

    public long getDuration() {
        return duration.get();
    }

    public Queue setDuration(final Long duration) {
        this.duration.set(duration);
        return this;
    }

    public int getSongCount() {
        return songCount.get();
    }

    public Queue setSongCount(final Integer songCount) {
        this.songCount.set(songCount);
        return this;
    }

    public Integer getPosition() {
        return position.get();
    }

    /**
     * @since 0.2.2
     */
    public Queue setPosition(final Integer position) {
        if (songs.isEmpty()) {
            this.position.set(-1);
            return this;
        }

        if (position >= songs.size() || position < 0) {
            log(LogMessageFX.FINE_QUEUE_POSITION_OUT_OF_BOUNDS, position, songs.size());
            return this;
        }

        this.positionListeners.forEach((listener) -> listener.changed(this.position.asObject(), this.getPosition(), position));

        this.position.set(position);

        return this;
    }

    /**
     * @since 0.2.2
     */
    public void addPositionListener(final ValueListener<Integer> change) {
        this.positionListeners.add(change);
    }
}
