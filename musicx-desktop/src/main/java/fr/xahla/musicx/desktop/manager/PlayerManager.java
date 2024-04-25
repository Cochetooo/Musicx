package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.listener.mediaPlayer.*;
import fr.xahla.musicx.desktop.logging.ErrorMessage;
import fr.xahla.musicx.desktop.model.Player;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.model.enums.RepeatMode;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.scene.media.Media;
import javafx.scene.media.MediaPlayer;
import javafx.util.Duration;

import java.io.File;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.settings;

/** <b>Class that allow views to use Player model, while keeping a protection layer to its usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class PlayerManager {

    private final QueueManager queueManager;
    private final Player player;

    private final List<MediaPlayListener> playListeners;
    private final List<MediaPauseListener> pauseListeners;
    private final List<MediaMuteListener> muteListeners;
    private final List<MediaCurrentTimeListener> currentTimeListeners;
    private final List<MediaChangeListener> changeListeners;

    private MediaPlayer mediaPlayer;

    public PlayerManager() {
        this.queueManager = new QueueManager();
        this.player = new Player();

        this.playListeners = new ArrayList<>();
        this.pauseListeners = new ArrayList<>();
        this.muteListeners = new ArrayList<>();
        this.currentTimeListeners = new ArrayList<>();
        this.changeListeners = new ArrayList<>();

        this.queueManager.onChangePosition((change, oldValue, newValue) -> {
            this.player.setSong(this.queueManager.getSongAt(newValue));
            this.updateSong();
            this.changeListeners.forEach((listener) -> listener.onChange(this.player.getSong()));
        });
    }

    // --- Queue Management ---

    /**
     * Remove all songs from the queue.
     */
    public void clearQueue() {
        this.queueManager.clear();
    }

    /**
     * Add a song to the queue at its last position.
     */
    public void queueLast(final Song song) {
        this.queueManager.queueLast(song);
    }

    /**
     * Add a song to the queue at the current position + 1.
     */
    public void queueNext(final Song song) {
        this.queueManager.queueNext(song);
    }

    /**
     * Set the current song by its position in the queue.
     */
    public void setCurrentSongByPosition(int position) {
        this.queueManager.setPosition(position);
    }

    public void setQueue(final List<Song> songs, int position) {
        this.queueManager.setQueue(songs, position);
    }

    public Duration getTotalQueueDuration() {
        return Duration.seconds(this.queueManager.getTotalDuration());
    }

    /**
     * Create media player, set up listeners and play the song
     */
    public void updateSong() {
        final var filepath = this.player.getSong().getFilePath();

        // Check if filepath exists
        final var existingFile = Files.exists(Paths.get(filepath));

        if (!existingFile) {

            return;
        }

        // Stop previous media if already playing
        if (!this.isPlayerInactive()) {
            this.mediaPlayer.stop();
        }

        final var audioFile = new File(this.player.getSong().getFilePath());
        final var media = new Media(audioFile.toURI().toString());

        this.mediaPlayer = new MediaPlayer(media);
        this.mediaPlayer.volumeProperty().bind(this.player.volumeProperty());

        this.mediaPlayer.setOnEndOfMedia(this::next);
        this.mediaPlayer.setOnPlaying(() -> playListeners.forEach(MediaPlayListener::onPlay));
        this.mediaPlayer.setOnPaused(() -> pauseListeners.forEach(MediaPauseListener::onPause));
        this.muteListeners.forEach((muteListener)
            -> this.mediaPlayer.muteProperty().addListener(muteListener::onToggleMute));
        this.currentTimeListeners.forEach((currentTimeListener)
            -> this.mediaPlayer.currentTimeProperty().addListener(currentTimeListener::onCurrentTimeChange));

        this.mediaPlayer.play();
    }

    // --- Media Controls ---

    public void mute() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        this.mediaPlayer.setMute(!this.mediaPlayer.isMute());
    }

    public void next() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        switch (this.getRepeatMode()) {
            case NO_REPEAT -> this.queueManager.moveNext();
            case QUEUE_REPEAT -> {
                if (this.queueManager.isLastSong()) {
                    this.queueManager.setPosition(0);
                } else {
                    this.queueManager.moveNext();
                }
            }
            case SONG_REPEAT -> this.queueManager.repeatSong();
        }
    }

    public void pause() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        if (settings().isSmoothFadeStop()) {
            final var startVolume = this.getVolume();

            DurationHelper.fade(
                Duration.seconds(0.25),
                startVolume,
                0.0,
                16,
                this::setVolume,
                this::getVolume,
                () -> {
                    this.mediaPlayer.pause();
                    this.setVolume(startVolume);
                }
            );
        } else {
            this.mediaPlayer.pause();
        }
    }

    public void previous() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        this.queueManager.movePrevious();
    }

    public void remove(final int position) {
        this.queueManager.remove(position);
    }

    public void resume() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        this.mediaPlayer.play();
    }

    public void seek(final Double seconds) {
        this.mediaPlayer.seek(Duration.millis(seconds));
    }

    public void shuffle() {
        this.queueManager.shuffle();
    }

    public void stop() {
        if (!this.isPlayerInactive()) {
            logger().info(ErrorMessage.MEDIA_PLAYER_NOT_CREATED_STOP.getMsg());
            this.mediaPlayer.stop();
        }

        this.queueManager.clear();
    }

    public void togglePlaying() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return;
        }

        if (this.isPlaying()) {
            this.pause();
            return;
        }

        this.resume();
    }

    public void toggleRepeat() {
        switch (this.player.getRepeatMode()) {
            case NO_REPEAT -> this.player.setRepeatMode(RepeatMode.QUEUE_REPEAT);
            case QUEUE_REPEAT -> this.player.setRepeatMode(RepeatMode.SONG_REPEAT);
            case SONG_REPEAT -> this.player.setRepeatMode(RepeatMode.NO_REPEAT);
        }
    }

    // --- Accessors / Modifiers ---

    public ObservableList<Song> getSongs() {
        return this.queueManager.getSongs();
    }

    public RepeatMode getRepeatMode() {
        return this.player.getRepeatMode();
    }

    public boolean isMuted() {
        return this.mediaPlayer.isMute();
    }

    /**
     * @return true if the media player has not yet been created, false otherwise.
     */
    public boolean isPlayerInactive() {
        return null == this.mediaPlayer;
    }

    public boolean isPlaying() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return false;
        }

        return this.mediaPlayer.getStatus() == MediaPlayer.Status.PLAYING;
    }

    public double getPlayingTime() {
        if (this.isPlayerInactive()) {
            logger().warning(ErrorMessage.MEDIA_PLAYER_NOT_CREATED.getMsg());
            return -1;
        }

        return this.mediaPlayer.getCurrentTime().toMillis();
    }

    public void setVolume(final Double volume) {
        this.player.setVolume(volume);
    }

    public double getVolume() {
        return this.player.getVolume();
    }

    public Duration getTotalDuration() {
        return this.mediaPlayer.getTotalDuration();
    }

    public Duration getCurrentTime() {
        return this.mediaPlayer.getCurrentTime();
    }

    public Song getCurrentSong() {
        return this.player.getSong();
    }

    // --- Event / Listeners ---

    public void onQueueChange(final ListChangeListener<Song> change) {
        this.getSongs().addListener(change);
    }

    public void onSongChange(final MediaChangeListener changeListener) {
        this.changeListeners.add(changeListener);
    }

    public void onCurrentTimeChange(final MediaCurrentTimeListener currentTimeListener) {
        this.currentTimeListeners.add(currentTimeListener);
    }

    public void onMute(final MediaMuteListener muteListener) {
        this.muteListeners.add(muteListener);
    }

    public void onPlay(final MediaPlayListener playListener) {
        this.playListeners.add(playListener);
    }

    public void onPause(final MediaPauseListener pauseListener) {
        this.pauseListeners.add(pauseListener);
    }

}
