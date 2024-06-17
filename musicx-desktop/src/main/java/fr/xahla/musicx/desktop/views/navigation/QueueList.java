package fr.xahla.musicx.desktop.views.navigation;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.input.MouseEvent;
import javafx.scene.paint.Color;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the queue list shown on the right-side container
 * @author Cochetooo
 * @since 0.2.2
 */
public class QueueList implements Initializable {

    @FXML private ListView<Song> queueListView;
    @FXML private Label queueInfoLabel;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
        this.queueListView.setItems(audioPlayer().getSongs());

        this.updateLabelText();
        audioPlayer().onQueueChange(change -> updateLabelText());

        this.queueListView.setCellFactory(list -> new ListCell<>() {
            @Override public void updateItem(final Song song, final boolean empty) {
                super.updateItem(song, empty);

                if (empty || null == song) {
                    this.setText(null);
                    return;
                }

                this.setTextFill(Color.WHITE);

                this.setText((this.getIndex() + 1) + ". " + song.getTitle());
            }
        });
    }

    @FXML public void play(final MouseEvent mouseEvent) {
        if (2 == mouseEvent.getClickCount()) {
            audioPlayer().setCurrentSongByPosition(queueListView.getSelectionModel().getSelectedIndex());
        }
    }

    @FXML public void remove() {
        audioPlayer().remove(queueListView.getSelectionModel().getSelectedIndex());
    }

    @FXML private void close() {
        scene().getNavContent().close();
    }

    private void updateLabelText() {
        queueInfoLabel.setText(audioPlayer().getSongs().size() + " " + resourceBundle.getString("queueList.infoLabel")
            + " - " + DurationHelper.getTimeString(audioPlayer().getTotalQueueDuration()));
    }
}
