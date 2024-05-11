import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.database.HibernateLoader;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.model.data.LibraryInterface;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import fr.xahla.musicx.domain.repository.data.LibraryRepositoryInterface;
import fr.xahla.musicx.domain.service.genreList.ImportGenresFromJson;
import fr.xahla.musicx.domain.service.localAudioFile.ImportSongsFromFolders;

import java.time.LocalDate;
import java.util.List;
import java.util.logging.Logger;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.ArtistRepository.artistRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.domain.repository.LabelRepository.labelRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public class MainTest {

    static class Context extends AbstractContext {
        protected Context() {
            super(Logger.getLogger(Context.class.getName()), new SettingsInterface() {
            }, new AudioPlayerManagerInterface() {
                @Override public double getCurrentTime() {
                    return 0;
                }

                @Override public RepeatMode getRepeatMode() {
                    return null;
                }

                @Override public ShuffleMode getShuffleMode() {
                    return null;
                }

                @Override public double getVolume() {
                    return 0;
                }

                @Override public boolean isMuted() {
                    return false;
                }

                @Override public boolean isPlaying() {
                    return false;
                }

                @Override public void setRepeatMode(final RepeatMode repeatMode) {

                }

                @Override public void setShuffleMode(final ShuffleMode shuffleMode) {

                }

                @Override public void setVolume(final double volume) {

                }

                @Override public void mute() {

                }

                @Override public void next() {

                }

                @Override public void pause() {

                }

                @Override public void previous() {

                }

                @Override public void resume() {

                }

                @Override public void seek(final double seconds) {

                }

                @Override public void stop() {

                }

                @Override public void togglePlaying() {

                }

                @Override public void toggleRepeat() {

                }

                @Override public void toggleShuffle() {

                }

                @Override public void updateSong() {

                }
            }, new LibraryManagerInterface() {
                @Override public LibraryInterface get() {
                    return null;
                }

                @Override public LibraryRepositoryInterface getRepository() {
                    return null;
                }

                @Override public void set(final LibraryInterface library) {

                }
            });
        }
    }

    public static void main(final String[] args) {
        new Context();
        new HibernateLoader();

       logger().info("Fetching all songs: ");

       final var time = System.currentTimeMillis();

       final var songs = songRepository().findAll();

       logger().info("Time loading all songs: " + (System.currentTimeMillis() - time) + " ms (" + songs.size() + " elements loaded).");

    }

}
