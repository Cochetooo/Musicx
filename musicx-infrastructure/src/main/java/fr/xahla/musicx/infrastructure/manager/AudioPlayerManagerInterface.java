package fr.xahla.musicx.infrastructure.manager;

public interface AudioPlayerManagerInterface {

    // Controls

    void mute();
    void pause();
    void playNext();
    void playPrevious();
    void resume();
    void seek(final Double seconds);
    void stop();
    void togglePlaying();
    void toggleRepeat();
    void toggleShuffle();

    // Getters

    boolean isPlaying();
    double getPlayingTime();
    double getTotalDuration();

}
