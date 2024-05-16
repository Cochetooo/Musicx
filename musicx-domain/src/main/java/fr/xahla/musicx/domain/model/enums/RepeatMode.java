package fr.xahla.musicx.domain.model.enums;

/**
 * List of all different mode of repetition for the track list.
 * @author Cochetooo
 */
public enum RepeatMode {

    /**
     * Do not repeat when the queue has no song left.
     */
    NO_REPEAT,

    /**
     * Repeat the entire queue when the queue has no song left.
     */
    QUEUE_REPEAT,

    /**
     * Repeat the current song.
     */
    SONG_REPEAT

}
