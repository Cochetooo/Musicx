package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.api.data.PlayerInterface;
import fr.xahla.musicx.desktop.model.PlayerViewModel;
import fr.xahla.musicx.desktop.model.QueueViewModel;
import fr.xahla.musicx.desktop.model.SongViewModel;
import javafx.scene.media.Media;
import javafx.scene.media.MediaPlayer;
import javafx.util.Duration;

import java.io.File;
import java.util.List;

/**
 * This class handles the queue and the current song playing.
 */
public class PlayerViewManager implements PlayerInterface {

    private final QueueViewModel queue;
    private final PlayerViewModel player;
    private final PlayerInterface callbackInterface;

    private MediaPlayer mediaPlayer;

    public PlayerViewManager(
        final QueueViewModel queue,
        final PlayerInterface callbackInterface
    ) {
        this.queue = queue;
        this.player = new PlayerViewModel();
        this.callbackInterface = callbackInterface;

        this.queue.songsProperty().addListener((observableValue, oldValue, newValue) -> {
            if (newValue.isEmpty()) {
                return;
            }

            this.player.setSong(newValue.getFirst());
        });
    }

    public void setQueue(List<SongViewModel> songs) {
        this.queue.setSongs(songs);
        this.play();
    }

    @Override public void play() {
        if (null != this.mediaPlayer) {
            this.stop();
        }

        var audioFile = new File(this.player.getSong().getFilePath());
        var media = new Media(audioFile.toURI().toString());

        this.mediaPlayer = new MediaPlayer(media);
        this.mediaPlayer.volumeProperty().bind(this.player.volumeProperty());
        this.mediaPlayer.setOnEndOfMedia(this::next);

        this.callbackInterface.play();

        this.mediaPlayer.play();
    }

    @Override public void previous() {

    }

    @Override public boolean togglePlaying() {
        if (null == this.mediaPlayer) {
            return false;
        }

        if (this.mediaPlayer.getStatus() == MediaPlayer.Status.PLAYING) {
            this.mediaPlayer.pause();
            return false;
        }

        this.mediaPlayer.play();
        return true;
    }

    @Override public void seek(Double seconds) {
        this.mediaPlayer.seek(new Duration(seconds));
        this.callbackInterface.seek(seconds);
    }

    @Override public void stop() {
        this.mediaPlayer.stop();
        this.queue.clear();
    }

    @Override public void next() {
        if (null == this.mediaPlayer) {
            return;
        }

        // We remove the current song in the queue, and then, if no song is there after (null), we stop the player.
        if (null == this.queue.next()) {
            this.stop();
        } else {
            this.play();
        }
    }

    @Override public void mute() {
        if (null == this.mediaPlayer) {
            return;
        }

        this.mediaPlayer.setMute(!this.mediaPlayer.isMute());
    }

    public QueueViewModel getQueue() {
        return this.queue;
    }

    public PlayerViewModel getPlayer() {
        return this.player;
    }

    public MediaPlayer getMediaPlayer() {
        return this.mediaPlayer;
    }
}
