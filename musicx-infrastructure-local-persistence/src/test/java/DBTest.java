import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.model.LibraryInterface;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;
import fr.xahla.musicx.domain.service.localAudioFile.ImportSongsFromFolders;
import fr.xahla.musicx.infrastructure.local.database.HibernateLoader;

import java.util.List;
import java.util.logging.Logger;

import static fr.xahla.musicx.infrastructure.local.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.infrastructure.local.repository.ArtistRepository.artistRepository;
import static fr.xahla.musicx.infrastructure.local.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.infrastructure.local.repository.LabelRepository.labelRepository;
import static fr.xahla.musicx.infrastructure.local.repository.SongRepository.songRepository;

public class DBTest {

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

    public static void main(String[] args) {
        final var context = new Context();
        final var hibernateLoader = new HibernateLoader();

        new ImportSongsFromFolders()
            .execute(List.of("C:\\Users\\hinoubli\\Music\\Kina Siern playlist"),
                List.of("mp3"),
                (progress, total) -> System.out.println("Progress: " + progress + " / " + total),
                songRepository(),
                albumRepository(),
                genreRepository(),
                artistRepository(),
                labelRepository()
            );
    }

}
