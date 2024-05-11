package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.input.MouseEvent;
import javafx.scene.paint.Color;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.player;

public class QueueList implements Initializable {

    @FXML private ListView<Song> queueListView;
    @FXML private Label queueInfoLabel;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.queueListView.setItems(player().getSongs());

        player().onQueueChange(change
            -> queueInfoLabel.setText(player().getSongs().size() + " " + resourceBundle.getString("queueList.infoLabel")
            + " - " + DurationHelper.getTimeString(player().getTotalQueueDuration()))
        );

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
            player().setCurrentSongByPosition(queueListView.getSelectionModel().getSelectedIndex());
        }
    }

    @FXML public void remove() {
        player().remove(queueListView.getSelectionModel().getSelectedIndex());
    }
}
