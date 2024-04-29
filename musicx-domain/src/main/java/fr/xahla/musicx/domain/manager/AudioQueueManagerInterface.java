package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.domain.model.AudioQueueInterface;

import java.util.List;

public interface AudioQueueManagerInterface {

    void addLast(final SongInterface song);
    void addNext(final SongInterface song);

    void clear();

    AudioQueueInterface get();
    List<? extends SongInterface> getSongs();
    long getTotalDuration();
    SongInterface getSongAt(final int index);

    boolean isLastSong();

    void remove(final int position);

    void setPosition(final int position);
    void setSongs(final List<? extends SongInterface> songs);

    void shuffle();

}
