package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.interfaces.ValueListener;
import fr.xahla.musicx.desktop.model.Queue;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;

import java.util.Collections;
import java.util.List;
import java.util.concurrent.atomic.AtomicLong;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Manages audio queue related list.
 * @author Cochetooo
 * @since 0.2.0
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

    /**
     * @since 0.2.0
     */
    public void setQueue(final List<Song> songs, int position) {
        this.queue.getSongs().setAll(songs);
        this.queue.setPosition(position);
    }

    /**
     * @since 0.2.0
     */
    public void clear() {
        this.queue.getSongs().clear();
        this.queue.setPosition(-1);
    }

    /**
     * @since 0.2.0
     */
    public void remove(final int position) {
        if (position < this.queue.getPosition()) {
            return;
        }

        if (position == this.queue.getPosition()) {
            this.moveNext();
        }

        this.queue.getSongs().remove(position);
    }

    /**
     * @since 0.2.0
     */
    public void setPosition(final Integer position) {
        this.queue.setPosition(position);
    }

    /**
     * @since 0.2.0
     */
    public void movePrevious() {
        this.queue.setPosition(this.queue.getPosition() - 1);
    }

    /**
     * @since 0.2.0
     */
    public void moveNext() {
        this.queue.setPosition(this.queue.getPosition() + 1);
    }

    /**
     * @since 0.2.0
     */
    public void queueNext(final Song song) {
        this.queue.getSongs().add(this.queue.getPosition() + 1, song);
    }

    /**
     * @since 0.2.0
     */
    public void queueLast(final Song song) {
        this.queue.getSongs().add(song);
    }

    /**
     * @since 0.2.0
     */
    public void repeatSong() {
        this.queue.setPosition(this.queue.getPosition());
    }

    /**
     * @since 0.2.0
     */
    public void shuffle() {
        Collections.shuffle(this.queue.getSongs());
    }

    /**
     * @return The song at the chosen index, otherwise <b>null</b> if index is out of bounds.
     * @since 0.2.0
     */
    public Song getSongAt(final int index) {
        if (index < 0 || index >= this.queue.getSongCount()) {
            logger().fine("QUEUE_SONG_OUT_OF_BOUNDS", index, queue.getPosition());
            return null;
        }

        return this.queue.getSongs().get(index);
    }

    /**
     * @since 0.2.0
     */
    public ObservableList<Song> getSongs() {
        return this.queue.getSongs();
    }

    /**
     * @since 0.2.0
     */
    public long getTotalDuration() {
        return this.queue.getDuration();
    }

    /**
     * @since 0.2.0
     */
    public boolean isLastSong() {
        return this.queue.getPosition() == this.getSongs().size();
    }

    /**
     * @since 0.2.0
     */
    public void onChangePosition(final ValueListener<Integer> change) {
        this.queue.addPositionListener(change);
    }
}
