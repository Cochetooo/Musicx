package fr.xahla.musicx.desktop.service.save;

import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.service.saveLocalSongs.WriteMetadataToAudioFile;
import javafx.application.Platform;
import javafx.concurrent.Task;

import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * Edit or create a song
 *
 * @author Cochetooo
 * @since 0.4.1
 */
public final class EditSongService {

    /**
     * @param callback Method called after the saving has been done, in a JavaFX Thread (Platform.runLater())
     */
    public void execute(final Song song, final CallbackInterface callback) {
        final var task = new Task<Void>() {
            @Override protected Void call() throws Exception {
                final var dto = song.getDto();

                // Persistence to database
                songRepository().save(dto);

                // Write to audio file
                if (null != song.getFilepath()) {
                    new WriteMetadataToAudioFile().execute(dto);
                }

                Platform.runLater(callback::done);
                return null;
            }
        };

        new Thread(task).start();
    }

}
