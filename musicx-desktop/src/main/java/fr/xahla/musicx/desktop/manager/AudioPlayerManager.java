package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.helper.animation.Animation;
import fr.xahla.musicx.desktop.model.AudioPlayer;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import javafx.beans.Observable;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.value.ObservableValue;
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

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Manages audio player interactions.
 * @author Cochetooo
 * @since 0.2.0
 */
public class AudioPlayerManager {

    private final QueueManager queueManager;
    private final AudioPlayer player;
    private final ObjectProperty<Song> editedSong;

    private final List<MediaPlayListener> playListeners;
    private final List<MediaPauseListener> pauseListeners;
    private final List<MediaMuteListener> muteListeners;
    private final List<MediaCurrentTimeListener> currentTimeListeners;
    private final List<MediaChangeListener> changeListeners;

    private MediaPlayer mediaPlayer;

    public AudioPlayerManager() {
        this.queueManager = new QueueManager();
        this.player = new AudioPlayer();

        this.editedSong = new SimpleObjectProperty<>();

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
     * @since 0.2.0
     */
    public void clearQueue() {
        this.queueManager.clear();
    }

    /**
     * Add a song to the queue at its last position.
     * @since 0.2.0
     */
    public void queueLast(final Song song) {
        this.queueManager.queueLast(song);
    }

    /**
     * Add a song to the queue at the current position + 1.
     * @since 0.2.0
     */
    public void queueNext(final Song song) {
        this.queueManager.queueNext(song);
    }

    /**
     * Set the current song by its position in the queue.
     * @since 0.2.0
     */
    public void setCurrentSongByPosition(int position) {
        this.queueManager.setPosition(position);
    }

    /**
     * @since 0.2.0
     */
    public void setQueue(final List<Song> songs, int position) {
        this.queueManager.setQueue(songs, position);
    }

    /**
     * @since 0.2.0
     */
    public Duration getTotalQueueDuration() {
        return Duration.millis(this.queueManager.getTotalDuration());
    }

    /**
     * Create media player, set up listeners and play the song
     * @since 0.2.0
     */
    public void updateSong() {
        final var filepath = this.player.getSong().getFilepath();

        // Check if filepath exists
        final var existingFile = Files.exists(Paths.get(filepath));

        if (!existingFile) {

            return;
        }

        // Stop previous media if already playing
        if (!this.isPlayerInactive()) {
            this.mediaPlayer.stop();
        }

        final var audioFile = new File(this.player.getSong().getFilepath());
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

    /**
     * @since 0.2.0
     */
    public void mute() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return;
        }

        this.mediaPlayer.setMute(!this.mediaPlayer.isMute());
    }

    /**
     * @since 0.2.0
     */
    public void next() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
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

    /**
     * @since 0.2.0
     */
    public void pause() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return;
        }

        if (scene().getSettings().getSmoothPause()) {
            final var startVolume = this.getVolume();

            new Animation(
                Duration.seconds(0.35),
                startVolume,
                0.0,
                16,
                this::getVolume,
                this::setVolume,
                () -> {
                    this.mediaPlayer.pause();
                    this.setVolume(startVolume);
                }
            ).play();
        } else {
            this.mediaPlayer.pause();
        }
    }

    /**
     * @since 0.2.0
     */
    public void previous() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return;
        }

        this.queueManager.movePrevious();
    }

    /**
     * @since 0.2.0
     */
    public void remove(final int position) {
        this.queueManager.remove(position);
    }

    /**
     * @since 0.2.0
     */
    public void resume() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return;
        }

        this.mediaPlayer.play();
    }

    /**
     * @since 0.2.0
     */
    public void seek(final Double seconds) {
        this.mediaPlayer.seek(Duration.millis(seconds));
    }

    /**
     * @since 0.2.0
     */
    public void shuffle() {
        this.queueManager.shuffle();
    }

    /**
     * @since 0.2.0
     */
    public void stop() {
        if (!this.isPlayerInactive()) {
            this.mediaPlayer.stop();
        }

        this.queueManager.clear();
    }

    /**
     * @since 0.2.0
     */
    public void togglePlaying() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return;
        }

        if (this.isPlaying()) {
            this.pause();
            return;
        }

        this.resume();
    }

    /**
     * @since 0.2.0
     */
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
     * @return True if the media player has not yet been created, false otherwise.
     */
    public boolean isPlayerInactive() {
        return null == this.mediaPlayer;
    }

    public boolean isPlaying() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
            return false;
        }

        return this.mediaPlayer.getStatus() == MediaPlayer.Status.PLAYING;
    }

    public double getPlayingTime() {
        if (this.isPlayerInactive()) {
            logger().warn("MEDIA_PLAYER_NOT_CREATED");
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

    public Song getEditedSong() {
        return this.editedSong.get();
    }

    public void setEditedSong(final Song song) {
        this.editedSong.set(song);
    }

    // --- Event / Listeners ---

    /**
     * @since 0.2.0
     */
    public void onQueueChange(final ListChangeListener<Song> change) {
        this.getSongs().addListener(change);
    }

    /**
     * @since 0.2.0
     */
    public void onSongChange(final MediaChangeListener changeListener) {
        this.changeListeners.add(changeListener);
    }

    /**
     * @since 0.2.0
     */
    public void onCurrentTimeChange(final MediaCurrentTimeListener currentTimeListener) {
        this.currentTimeListeners.add(currentTimeListener);
    }

    /**
     * @since 0.2.0
     */
    public void onMute(final MediaMuteListener muteListener) {
        this.muteListeners.add(muteListener);
    }

    /**
     * @since 0.2.0
     */
    public void onPlay(final MediaPlayListener playListener) {
        this.playListeners.add(playListener);
    }

    /**
     * @since 0.2.0
     */
    public void onPause(final MediaPauseListener pauseListener) {
        this.pauseListeners.add(pauseListener);
    }

    // Interfaces

    public interface MediaChangeListener {
        void onChange(final Song song);
    }

    public interface MediaCurrentTimeListener {
        void onCurrentTimeChange(
            final ObservableValue<? extends Duration> observable,
            final Duration oldValue,
            final Duration newValue
        );
    }

    public interface MediaMuteListener {
        void onToggleMute(Observable observable);
    }

    public interface MediaPauseListener {
        void onPause();
    }

    public interface MediaPlayListener {
        void onPlay();
    }

}
