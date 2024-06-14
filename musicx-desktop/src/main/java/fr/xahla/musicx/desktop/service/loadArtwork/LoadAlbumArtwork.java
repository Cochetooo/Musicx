package fr.xahla.musicx.desktop.service.loadArtwork;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.helper.StringHelper;
import javafx.application.Platform;
import javafx.concurrent.Task;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;

import static fr.xahla.musicx.domain.application.AbstractContext.albumRepository;
import static fr.xahla.musicx.domain.service.apiHandler.ItunesApiHandler.itunesApi;
import static fr.xahla.musicx.domain.service.apiHandler.LastFmApiHandler.lastFmApi;

/**
 * @author Cochetooo
 * @since 0.3.3
 */
public class LoadAlbumArtwork {

    private TaskCallback callback;

    public void execute(
        final ImageView imageView,
        final Song song,
        final TaskCallback callback
    ) {
        this.callback = callback;
        new Thread(getTask(imageView, song)).start();
    }

    private Task<Void> getTask(
        final ImageView imageView,
        final Song song
    ) {
        return new Task<>() {
            @Override protected Void call() {
                final var album = song.getAlbum().getDto();

                // If no album we return
                if (null == album) {
                    return null;
                }

                // If album artwork is null or blank, we fetch image with APIs
                if (StringHelper.str_is_null_or_blank(album.getArtworkUrl())) {
                    fetchDtoArtwork(album);

                    // If APIs proceed to find an artwork
                    if (!StringHelper.str_is_null_or_blank(album.getArtworkUrl())) {
                        albumRepository().save(album);
                        song.getAlbum().setArtworkUrl(album.getArtworkUrl());
                    }
                }

                Platform.runLater(() -> imageView.setImage(song.getAlbum().getImage()));
                callback.onSuccess(song.getAlbum().getImage());

                return null;
            }
        };
    }

    private void fetchDtoArtwork(final AlbumDto album) {
        // We try to get the artwork from LastFM then from iTunes if not found
        lastFmApi().fetchAlbumFromExternal(album, false);

        if (StringHelper.str_is_null_or_blank(album.getArtworkUrl())) {
            itunesApi().fetchAlbumFromExternal(album, false);
        }
    }

    // --- Classes ---

    public interface TaskCallback {
        void onSuccess(final Image image);
    }

}
