package fr.xahla.musicx.desktop.service.save;

import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.service.saveLocalSongs.WriteMetadataToAudioFile;
import javafx.application.Platform;
import javafx.concurrent.Task;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.albumRepository;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * Edit or create an album.
 * @author Cochetooo
 * @since 0.4.1
 */
public final class EditAlbumService {

    /**
     * @param callback Method called after the saving has been done, in a JavaFX Thread (Platform.runLater())
     */
    public void execute(final Album album, final CallbackInterface callback) {
        final var task = new Task<Void>() {
            @Override protected Void call() throws Exception {
                final var dto = album.getDto();

                // Persistence to database
                albumRepository().save(dto);

                // Write to audio file
                new WriteMetadataToAudioFile().execute(dto);

                Platform.runLater(() -> {
                    updateSongs(album);
                    callback.done();
                });
                return null;
            }
        };

        new Thread(task).start();
    }

    private void updateSongs(final Album album) {
        final var songs = scene().getLocalLibraryScene().getLibrary().getLocalSongs().stream()
            .filter(song -> null != song && song.getAlbum().getId() == album.getId())
            .toList();

        songs.forEach(song -> song.getAlbum()
            .setArtworkUrl(album.getArtworkUrl())
            .setCatalogNo(album.getCatalogNo())
            .setDiscTotal(album.getDiscTotal())
            .setName(album.getName())
            .setReleaseDate(album.getReleaseDate())
            .setTrackTotal(album.getTrackTotal())
            .setType(album.getType())
        );
    }

}
