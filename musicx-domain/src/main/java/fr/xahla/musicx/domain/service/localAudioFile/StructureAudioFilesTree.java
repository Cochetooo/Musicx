package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.domain.logging.LogMessage;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.nio.file.attribute.FileAttribute;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.*;

/**
 * Service that collects all songs and restructure them in an organized folder with Artist > Album > Songs.
 * @author Cochetooo
 * @since 0.3.2
 */
public class StructureAudioFilesTree {

    private static final String UNKNOWN_ARTISTS = "Unknown Artists";
    private String rootFolder;

    /**
     * @since 0.3.2
     */
    public void execute(final String rootFolder) {
        final var artists = artistRepository().findAll();
        this.rootFolder = rootFolder;

        artists.forEach(this::createArtist);
    }

    private void createArtist(final ArtistDto artist) {
        final var artistPath = Path.of(rootFolder).resolve(artist.getName());

        try {
            if (!Files.exists(artistPath)) {
                Files.createDirectory(artistPath);
            }

            final var albums = artistRepository().getAlbums(artist);

            albums.forEach((album) -> this.createAlbum(album, artistPath));
        } catch (final IOException exception) {
            error(exception, LogMessage.ERROR_IO_FOLDER_CREATE, artistPath.toAbsolutePath());
        }
    }

    private void createAlbum(final AlbumDto album, final Path artistPath) {
        final var albumPath = artistPath.resolve(
            album.getReleaseDate().getYear() + " - " + album.getName()
        );

        try {
            if (!Files.exists(albumPath)) {
                Files.createDirectory(albumPath);
            }

            final var songs = albumRepository().getSongs(album);

            songs.forEach((song) -> this.createSong(song, albumPath));
        } catch (final IOException exception) {
            error(exception, LogMessage.ERROR_IO_FOLDER_CREATE, albumPath.toAbsolutePath());
        }
    }

    private void createSong(final SongDto song, final Path albumPath) {
        try {
            final var songPath = albumPath.resolve(
                String.format("%02d", song.getTrackNumber()) + " - " + song.getTitle() + ".mp3"
            );

            logger().finest("Song path : " + songPath);

            if (!Files.exists(songPath)) {
                Files.copy(
                    Paths.get(song.getFilepath()),
                    songPath,
                    StandardCopyOption.REPLACE_EXISTING
                );

                if (!Files.exists(songPath)) {
                    log(LogMessage.WARNING_IO_FILE_COPY, song.getFilepath(), songPath);
                } else {
                    log(LogMessage.FINE_IO_FILE_COPIED, song.getFilepath(), songPath);
                }
            }
        } catch (final IOException exception) {
            error(exception, LogMessage.ERROR_IO_FILE_MOVE, "song " + song.getFilepath(), albumPath);
        }
    }

}
