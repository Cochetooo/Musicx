package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;

import java.time.LocalDate;
import java.util.Comparator;
import java.util.List;

/**
 * Helper that brings all kind of sorting for collections.
 * @author Cochetooo
 * @since 0.4.0
 */
public final class CollectionSortHelper {

    public static void sortSongsByYears(final List<Song> songs) {
        songs.sort(
            Comparator.comparing(obj -> {
                    final var song = (Song) obj;
                    return (null == song.getAlbum()) ? LocalDate.MIN : song.getAlbum().getReleaseDate();
                })
                .thenComparing(obj -> {
                    final var song = (Song) obj;
                    return null == song.getAlbum() ? "" : song.getAlbum().getName();
                })
                .thenComparing(obj -> {
                    final var song = (Song) obj;
                    return song.getDiscNumber();
                })
                .thenComparing(obj -> {
                    final var song = (Song) obj;
                    return song.getTrackNumber();
                })
                .thenComparing(obj -> {
                    final var song = (Song) obj;
                    return song.getTitle();
                })
        );
    }

    public static void sortAlbumsByYears(final List<Album> albums) {
        albums.sort(
            Comparator.comparing(Album::getReleaseDate)
                .thenComparing(album -> null == album.getArtist() ? "" : album.getArtist().getName())
                .thenComparing(Album::getName)
        );
    }

}
