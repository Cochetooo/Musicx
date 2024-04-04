package fr.xahla.musicx.desktop.model;

import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.atomic.AtomicLong;

public class QueueViewModel {

    private ListProperty<SongViewModel> songs;
    private LongProperty duration;
    private IntegerProperty songCount;

    public QueueViewModel() {
        this.songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.duration = new SimpleLongProperty();
        this.songCount = new SimpleIntegerProperty();

        this.songs.addListener((ListChangeListener<SongViewModel>) change -> {
            songCount.set(change.getList().size());

            AtomicLong totalDuration = new AtomicLong(0L);
            change.getList().forEach((song) -> totalDuration.addAndGet(song.getDuration()));

            duration.set(totalDuration.get());
        });
    }

    public SongViewModel next() {
        this.songs.removeFirst();
        return this.current();
    }

    public SongViewModel current() {
        if (0 == this.songs.getSize()) {
            return null;
        }

        return this.songs.getFirst();
    }

    public void clear() {
        this.songs.clear();
    }

    public void remove(int position) {
        this.songs.remove(position);
    }

    public void queueNext(SongViewModel song) {
        this.songs.add(1, song);
    }

    public void queueLast(SongViewModel song) {
        this.songs.add(song);
    }

    public ObservableList<SongViewModel> getSongs() {
        return songs.get();
    }

    public ListProperty<SongViewModel> songsProperty() {
        return this.songs;
    }

    public QueueViewModel setSongs(List<SongViewModel> songs) {
        this.getSongs().setAll(songs);
        return this;
    }

    public Long getDuration() {
        return duration.get();
    }

    public Integer getSongCount() {
        return songCount.get();
    }
}
