package fr.xahla.musicx.desktop.listener.mediaPlayer;

import javafx.beans.value.ObservableValue;
import javafx.util.Duration;

/** <b>Listener invoked when Audio Player's current track time has changed.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 * @see fr.xahla.musicx.desktop.manager.PlayerManager
 * @see javafx.scene.media.MediaPlayer
 */
public interface MediaCurrentTimeListener {

    void onCurrentTimeChange(
        final ObservableValue<? extends Duration> observable,
        final Duration oldValue,
        final Duration newValue
    );

}
