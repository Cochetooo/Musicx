package fr.xahla.musicx.api.data;

public interface PlayerInterface {

    /**
     * Play the current track of the queue.
     */
    void play();

    /**
     * Request to play the previous song in the queue.
     */
    void previous();

    /**
     * Request to resume / pause the song, depending on the current state of the player.
     * @return True if the song has been toggled to "playing" and False if it has been set to "paused".
     */
    boolean togglePlaying();

    /**
     * Request to stop the track and clear the queue.
     */
    void stop();

    /**
     * Request to play the next track of the queue, or stop the player if there's no track left in the queue.
     */
    void next();

    /**
     * Request to mute / unmute the song, depending on the state of the mute.
     */
    void mute();

    /**
     * Seek song time to specified seconds.
     */
    void seek(Double seconds);

}
