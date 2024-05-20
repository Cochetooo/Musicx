package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.model.entity.Artist;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.concurrent.Task;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

import static fr.xahla.musicx.desktop.context.DesktopContext.library;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Manages artist list in the navigation bar.
 * @author Cochetooo
 * @since 0.2.2
 */
@Deprecated
public class ArtistListManager {

    private final ListProperty<Artist> artists;
    private Thread getArtistListThread;

    public ArtistListManager() {
        if (null == library()) {
            logger().severe("Library has not been initialized when ArtistManager try to access it.");
            throw new RuntimeException();
        }

        artists = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.getArtistList(library().getSongs());

        library().onSongsChange(change -> {
            if (null != this.getArtistListThread && this.getArtistListThread.isAlive()) {
                logger().info("The thread for Artist List Get has been interrupted by a new call on this method.");
                this.getArtistListThread.interrupt();
            }

            final var getArtistListTask = new Task<>() {
                @Override protected Void call() {
                    @SuppressWarnings("unchecked")
                    final var list = (List<Song>) change.getList();
                    getArtistList(list);

                    return null;
                }
            };

            this.getArtistListThread = new Thread(getArtistListTask);
            this.getArtistListThread.start();
        });
    }

    public void getArtistList(final List<Song> songs) {
        final var artistList = new ArrayList<Artist>();

        songs.forEach((song) -> {
            if (null == song.getArtist()) {
                return;
            }

            if (artistList.stream()
                .map(Artist::getName)
                .map(String::toLowerCase)
                .toList()
                .contains(song.getArtist().getName().toLowerCase())) {
                return;
            }

            artistList.add(song.getArtist());
        });

        artistList.sort(Comparator.comparing(Artist::getName, String.CASE_INSENSITIVE_ORDER));
        this.artists.setAll(artistList);
    }

    public List<Song> getSongsFromArtist(
        final List<Song> songs,
        final Artist artist
    ) {
        final var songList = new ArrayList<Song>();

        songs.forEach((song) -> {
            if (null == song.getArtist()) {
                return;
            }

            if (song.getArtist().getName().equalsIgnoreCase(artist.getName())) {
                songList.add(song);
            }
        });

        final Comparator<Song> comparatorDate = Comparator.comparing(song -> song.getAlbum().getReleaseDate());
        final Comparator<Song> comparatorAlbum = Comparator.comparing(song -> song.getAlbum().getName());

        songList.sort(comparatorDate.thenComparing(comparatorAlbum));

        return songList;
    }

    // --- Accessors / Modifiers ---

    public ObservableList<Artist> getArtists() {
        return this.artists;
    }

    public void onArtistsChange(ListChangeListener<Artist> change) {
        this.artists.addListener(change);
    }

}
