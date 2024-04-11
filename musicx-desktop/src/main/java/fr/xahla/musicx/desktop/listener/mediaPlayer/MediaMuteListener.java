package fr.xahla.musicx.desktop.listener.mediaPlayer;

import javafx.beans.Observable;

/** <b>Listener invoked when Audio Player's media has been un/muted.</b>
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
public interface MediaMuteListener {

    void onToggleMute(Observable observable);

}
