package fr.xahla.musicx.infrastructure.model.data.enums;

/** <b>Enum containing all different kind of repeat modes for the audio player.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
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
