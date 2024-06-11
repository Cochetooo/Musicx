package fr.xahla.musicx.desktop.views.menus.submenus;

import javafx.fxml.FXML;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class AudioPlayerMenu {

    @FXML public void togglePlaying() {
        audioPlayer().togglePlaying();
    }

    @FXML public void previous() {
        audioPlayer().previous();
    }

    @FXML public void next() {
        audioPlayer().next();
    }

    @FXML public void mute() {
        audioPlayer().mute();
    }

}
