package fr.xahla.musicx.domain.service.structureLocalSongs;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.SongDto;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.FileHelper.file_get_extension;
import static fr.xahla.musicx.domain.helper.StringHelper.str_convert_to_path;

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
        final var artistPath = Path.of(rootFolder).resolve(
            str_convert_to_path(artist.getName())
        );

        try {
            if (!Files.exists(artistPath)) {
                Files.createDirectory(artistPath);
            }

            final var albums = artistRepository().getAlbums(artist);

            albums.forEach((album) -> this.createAlbum(album, artistPath));
        } catch (final IOException exception) {
            logger().error(exception, "IO_FOLDER_CREATE_ERROR", artistPath.toAbsolutePath());
        }
    }

    private void createAlbum(final AlbumDto album, final Path artistPath) {
        final var albumPath = artistPath.resolve(
            album.getReleaseDate().getYear() + " - "
                + str_convert_to_path(album.getName())
        );

        try {
            if (!Files.exists(albumPath)) {
                Files.createDirectory(albumPath);
            }

            final var songs = albumRepository().getSongs(album);

            songs.forEach((song) -> this.createSong(song, albumPath));
        } catch (final IOException exception) {
            logger().error(exception, "IO_FOLDER_CREATE_ERROR", albumPath.toAbsolutePath());
        }
    }

    private void createSong(final SongDto song, final Path albumPath) {
        try {
            final var songPath = albumPath.resolve(
                String.format("%02d", song.getTrackNumber()) + " - "
                    + str_convert_to_path(song.getTitle()) + "."
                    + file_get_extension(Path.of(song.getFilepath()))
            );

            logger().finest("Song path : " + songPath);

            if (!Files.exists(songPath)) {
                Files.copy(
                    Paths.get(song.getFilepath()),
                    songPath,
                    StandardCopyOption.REPLACE_EXISTING
                );

                if (!Files.exists(songPath)) {
                    logger().warn("IO_FILE_COPY_ERROR", song.getFilepath(), albumPath);
                } else {
                    logger().fine("IO_FILE_COPIED", song.getFilepath(), albumPath);
                }
            }
        } catch (final IOException exception) {
            logger().error(exception, "IO_FILE_MOVE_ERROR", song.getFilepath(), albumPath);
        }
    }

}
