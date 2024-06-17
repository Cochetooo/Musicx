package fr.xahla.musicx.desktop.factory;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.scene.control.TableCell;
import javafx.util.Duration;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class TrackDurationTableCell extends TableCell<Song, Long> {
    @Override protected void updateItem(final Long duration, final boolean empty) {
        if (null == duration || empty) {
            setText(null);
        } else {
            setText(DurationHelper.getTimeString(Duration.millis(duration)));
        }
    }
}
