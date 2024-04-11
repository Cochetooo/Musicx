package fr.xahla.musicx.desktop.listener.mediaPlayer;

import fr.xahla.musicx.desktop.model.entity.Song;

/** <b>Listener invoked when Audio Player's media is renewed.</b>
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
public interface MediaChangeListener {

    void onChange(final Song song);

}
