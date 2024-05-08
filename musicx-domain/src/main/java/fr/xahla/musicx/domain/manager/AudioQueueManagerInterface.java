package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.domain.model.data.AudioQueueInterface;

import java.util.List;

public interface AudioQueueManagerInterface {

    void addLast(final SongDto song);
    void addNext(final SongDto song);

    void clear();

    AudioQueueInterface get();
    List<? extends SongDto> getSongs();
    long getTotalDuration();
    SongDto getSongAt(final int index);

    boolean isLastSong();

    void remove(final int position);

    void setPosition(final int position);
    void setSongs(final List<? extends SongDto> songs);

    void shuffle();

}
