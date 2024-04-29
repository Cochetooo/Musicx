package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;

public interface AudioPlayerManagerInterface {

    double getCurrentTime();
    RepeatMode getRepeatMode();
    ShuffleMode getShuffleMode();
    double getVolume();

    boolean isMuted();
    boolean isPlaying();

    void setRepeatMode(final RepeatMode repeatMode);
    void setShuffleMode(final ShuffleMode shuffleMode);
    void setVolume(final double volume);

    void mute();
    void next();
    void pause();
    void previous();
    void resume();
    void seek(final double seconds);
    void stop();
    void togglePlaying();
    void toggleRepeat();
    void toggleShuffle();
    void updateSong();

}
