import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.model.LibraryInterface;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;
import fr.xahla.musicx.infrastructure.local.database.HibernateLoader;
import fr.xahla.musicx.infrastructure.local.model.AlbumEntity;
import fr.xahla.musicx.infrastructure.local.model.BandArtistEntity;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.logging.Logger;

import static fr.xahla.musicx.infrastructure.external.repository.ItunesApiHandler.itunesApi;
import static fr.xahla.musicx.infrastructure.external.repository.LastFmApiHandler.lastFmApi;
import static fr.xahla.musicx.infrastructure.local.repository.GenreRepository.genreRepository;

public class APITest {

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

        final var artist = new BandArtistEntity()
            .setName("Katatonia");

        final var album = new AlbumEntity()
            .setName("Last Fair Deal Gone Down")
            .setArtist(artist);

        itunesApi().fetchAlbumFromExternal(album, false);
        lastFmApi().fetchArtistFromExternal(artist, false);

        System.out.println("coubeh");
    }

}
