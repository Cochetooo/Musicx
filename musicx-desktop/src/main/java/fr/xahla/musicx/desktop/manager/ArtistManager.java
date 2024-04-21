package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.logging.ErrorMessage;
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

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;
import static fr.xahla.musicx.desktop.DesktopContext.library;

/** <b>Class that allow views to use Artist model, while keeping a protection layer to its usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class ArtistManager {

    private final ListProperty<Artist> artists;
    private Thread getArtistListThread;

    public ArtistManager() {
        if (null == library()) {
            logger().severe(ErrorMessage.LIBRARY_NOT_INITIALIZED.getMsg("ArtistManager"));
            throw new RuntimeException();
        }

        artists = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.getArtistList(library().getSongs());

        library().onSongsChange(change -> {
            if (null != this.getArtistListThread && this.getArtistListThread.isAlive()) {
                logger().info(ErrorMessage.THREAD_INTERRUPTED_INFO.getMsg("Artist List Get"));
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

        final Comparator<Song> comparatorYear = Comparator.comparingInt(song -> song.getAlbum().getReleaseYear());
        final Comparator<Song> comparatorAlbum = Comparator.comparing(song -> song.getAlbum().getName());

        songList.sort(comparatorYear.thenComparing(comparatorAlbum));

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
