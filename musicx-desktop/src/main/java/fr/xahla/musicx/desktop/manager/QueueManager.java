package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.listener.ValueListener;
import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.Queue;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;

import java.util.Collections;
import java.util.List;
import java.util.concurrent.atomic.AtomicLong;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/** <b>Class that allow views to use Queue model, while keeping a protection layer to its usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class QueueManager {

    private final Queue queue;

    public QueueManager() {
        this.queue = new Queue();

        this.queue.songsProperty().addListener((ListChangeListener<Song>) change -> {
            queue.setSongCount(change.getList().size());

            var totalDuration = new AtomicLong(0L);
            change.getList().forEach((song) -> totalDuration.addAndGet(song.getDuration()));

            queue.setDuration(totalDuration.get());
        });
    }

    public void setQueue(final List<Song> songs, int position) {
        this.queue.getSongs().setAll(songs);
        this.queue.setPosition(position);
    }

    public void clear() {
        this.queue.getSongs().clear();
        this.queue.setPosition(-1);
    }

    public void remove(final int position) {
        if (position < this.queue.getPosition()) {
            return;
        }

        if (position == this.queue.getPosition()) {
            this.moveNext();
        }

        this.queue.getSongs().remove(position);
    }

    public void setPosition(final Integer position) {
        this.queue.setPosition(position);
    }

    public void movePrevious() {
        this.queue.setPosition(this.queue.getPosition() - 1);
    }

    public void moveNext() {
        this.queue.setPosition(this.queue.getPosition() + 1);
    }

    public void queueNext(final Song song) {
        this.queue.getSongs().add(this.queue.getPosition() + 1, song);
    }

    public void queueLast(final Song song) {
        this.queue.getSongs().add(song);
    }

    public void repeatSong() {
        this.queue.setPosition(this.queue.getPosition());
    }

    public void shuffle() {
        Collections.shuffle(this.queue.getSongs());
    }

    public Song getSongAt(final int index) {
        if (index < 0 || index >= this.queue.getSongCount()) {
            logger().warning(ErrorMessage.QUEUE_SONG_OUT_OF_BOUNDS.getMsg(index, this.queue.getSongCount()));
            return null;
        }

        return this.queue.getSongs().get(index);
    }

    public ObservableList<Song> getSongs() {
        return this.queue.getSongs();
    }

    public long getTotalDuration() {
        return this.queue.getDuration();
    }

    public boolean isLastSong() {
        return this.queue.getPosition() == this.getSongs().size();
    }

    public void onChangePosition(final ValueListener<Integer> change) {
        this.queue.addPositionListener(change);
    }
}
