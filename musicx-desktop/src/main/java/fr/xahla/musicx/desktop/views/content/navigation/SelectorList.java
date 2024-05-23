package fr.xahla.musicx.desktop.views.content.navigation;

/**
 * View for the selected category sorting list panel.
 * @author Cochetooo
 * @since 0.2.0
 */
public class SelectorList {

    /* @FXML private ListView<Artist> selectorListView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.updateList();

        //artist().onArtistsChange(change -> this.updateList());

        this.selectorListView.setCellFactory(list -> new ListCell<>() {
            @Override public void updateItem(final Artist artist, final boolean empty) {
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
                    artistName.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, 15));
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
                audioPlayer().setQueue(trackList().getSongs(), 0);
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

        audioPlayer().setQueue(trackList().getSongs(), 0);
    }

    @FXML public void playShuffled() {
        this.playNow();
        audioPlayer().shuffle();
    }

    private void updateList() {
        logger().finest("UPDATE artist list.");

        final var artistList = FXCollections.observableList(artist().getArtists());
        artistList.addFirst(null);

        Platform.runLater(() -> this.selectorListView.setItems(artistList));
    } */
}
