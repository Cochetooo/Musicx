package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

/** <b>Class that defines the Queue model for a desktop app usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Queue {

    private final ListProperty<Song> songs;
    private final LongProperty duration;
    private final IntegerProperty songCount;
    private final IntegerProperty position;

    public Queue() {
        this.songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.duration = new SimpleLongProperty();
        this.songCount = new SimpleIntegerProperty();
        this.position = new SimpleIntegerProperty();
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

    public LongProperty durationProperty() {
        return duration;
    }

    public Queue setDuration(final Long duration) {
        this.duration.set(duration);
        return this;
    }

    public int getSongCount() {
        return songCount.get();
    }

    public IntegerProperty songCountProperty() {
        return songCount;
    }

    public Queue setSongCount(final Integer songCount) {
        this.songCount.set(songCount);
        return this;
    }

    public Integer getPosition() {
        return position.get();
    }

    public IntegerProperty positionProperty() {
        return position;
    }

    public Queue setPosition(final Integer position) {
        if (this.getSongs().isEmpty()) {
            this.position.set(-1);
            return this;
        }

        if (position >= this.getSongs().size() || position < 0) {
            logger().warning(ErrorMessage.QUEUE_POSITION_OUT_OF_BOUNDS.getMsg(position, this.getSongs().size()));
            return this;
        }

        this.position.set(position);
        return this;
    }
}
