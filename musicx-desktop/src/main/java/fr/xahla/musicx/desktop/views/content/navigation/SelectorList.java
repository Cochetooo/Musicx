package fr.xahla.musicx.desktop.views.content.navigation;

import fr.xahla.musicx.infrastructure.model.data.enums.SoftwareInfo;
import fr.xahla.musicx.desktop.model.entity.Artist;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.*;

/** <b>View for the selected category sorting list panel</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class SelectorList implements Initializable {

    @FXML private ListView<Artist> selectorListView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var artistList = FXCollections.observableList(artist().getArtists());
        artistList.addFirst(null);

        this.selectorListView.setItems(artistList);

        artist().onArtistsChange(change -> {
            final var updateArtistList = FXCollections.observableList(artist().getArtists());
            updateArtistList.addFirst(null);

            this.selectorListView.setItems(updateArtistList);
        });

        this.selectorListView.setCellFactory(list -> new ListCell<>() {
            @Override public void updateItem(final Artist artist, final boolean empty) {
                super.updateItem(artist, empty);

                if (empty || null == artist) {
                    if (0 == this.getIndex()) {
                        this.setTextFill(Color.WHITE);
                        this.setText("All Artists");
                    } else {
                        this.setText(null);
                    }

                    this.setGraphic(null);
                } else {
                    final var artistName = new Text(artist.getName());
                    artistName.setFont(Font.font(SoftwareInfo.APP_PRIMARY_FONT.getInfo(), FontWeight.BOLD, 15));
                    artistName.setFill(Color.LIGHTGRAY);

                    final var songCount = new Text(artist().getSongsFromArtist(library().getSongs(), artist).size() + " tracks");
                    songCount.setFont(Font.font(12));
                    songCount.setFill(Color.GRAY);

                    final var vBox = new VBox(artistName, songCount);
                    this.setGraphic(vBox);
                    this.setText(null);
                }
            }
        });

        this.selectorListView.setStyle(
            "-fx-font-weight: bold"
        );
    }

    @FXML private void onListClick(final MouseEvent event) {
        final var artist = this.selectorListView.getSelectionModel().getSelectedItem();

        if (MouseButton.PRIMARY == event.getButton()) {
            if (null == artist) {
                trackList().setQueue(library().getSongs(), 0);
            } else {
                trackList().setQueue(
                    artist().getSongsFromArtist(library().getSongs(), artist),
                    0
                );
            }

            if (2 == event.getClickCount()) {
                player().setQueue(trackList().getSongs(), 0);
            }
        }
    }

    @FXML public void playNow() {
        final var artist = this.selectorListView.getSelectionModel().getSelectedItem();

        if (null == artist) {
            trackList().setQueue(library().getSongs(), 0);
        } else {
            trackList().setQueue(
                artist().getSongsFromArtist(library().getSongs(), artist),
                0
            );
        }

        player().setQueue(trackList().getSongs(), 0);
    }

    @FXML public void playShuffled() {
        this.playNow();
        player().shuffle();
    }
}
