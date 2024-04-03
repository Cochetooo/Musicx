package fr.xahla.musicx.desktop.views.content.trackList;

import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.config.internationalization.Translator;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.domain.model.Song;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.infrastructure.persistence.repository.LibraryRepository;
import javafx.beans.binding.Bindings;
import javafx.beans.property.SimpleStringProperty;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.input.MouseEvent;

import java.time.Duration;
import java.util.ArrayList;

public class TrackListViewController implements ViewControllerInterface {

    public record Props(
        LibraryViewModel library
    ) implements ViewControllerProps {}

    @FXML private TableView<SongViewModel> trackList;

    @FXML private TableColumn<SongViewModel, Integer> trackListDurationColumn;
    @FXML private TableColumn<String, Integer> trackListNumberColumn;
    @FXML private TableColumn<SongViewModel, String> trackListExtensionColumn;
    @FXML private TableColumn<SongViewModel, String> trackListArtistNameColumn;
    @FXML private TableColumn<SongViewModel, String> trackListAlbumNameColumn;
    @FXML private TableColumn<SongViewModel, Integer> trackListAlbumYearColumn;

    @FXML public Label titleLabel;

    private Translator translator;

    private MusicxViewController parent;
    private Props props;

    private Tooltip formatNotSupported;

    @Override
    public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;
        this.props = (TrackListViewController.Props) props;

        trackList.setItems(this.props.library().getSongs());

        this.formatNotSupported = new Tooltip("library.error.formatNotSupported");

        this.makeNumberColumn();
        this.makeArtistColumns();
        this.makeDurationColumn();
        this.makeExtensionColumn();

        this.translator = new Translator("library");
        this.makeTranslations();
    }

    private void makeNumberColumn() {
        this.trackListNumberColumn.setCellValueFactory(cellData -> {
            var rowIndex = trackList.getItems().indexOf(cellData.getValue()) + 1;
            return Bindings.createObjectBinding(() -> rowIndex);
        });
    }

    private void makeArtistColumns() {
        this.trackListArtistNameColumn.setCellValueFactory((cellData) ->
            cellData.getValue().getArtist().nameProperty());

        this.trackListAlbumNameColumn.setCellValueFactory((cellData) ->
            cellData.getValue().getAlbum().nameProperty());

        this.trackListAlbumYearColumn.setCellValueFactory((cellData) ->
            cellData.getValue().getAlbum().releaseYearProperty().asObject());
    }

    private void makeDurationColumn() {
        this.trackListDurationColumn.setCellFactory(songDurationTableColumn -> new TableCell<>(){
            @Override
            protected void updateItem(Integer duration, boolean empty) {
                if (empty || duration == null) {
                    setText(null);
                } else {
                    setText(DurationHelper.getDurationFormatted(Duration.ofSeconds(duration)));
                }
            }
        });
    }

    private void makeExtensionColumn() {
        this.trackListExtensionColumn.setCellFactory(songExtensionTableColumn -> new TableCell<>(){
            @Override
            protected void updateItem(String filePath, boolean empty) {
                if (empty || filePath == null) {
                    setText(null);
                } else {
                    var extension = filePath.substring(filePath.lastIndexOf(".") + 1).toUpperCase();

                    this.setText(extension);

                    if (extension.equals("FLAC")) {
                        this.setStyle("-fx-background-color: -color-danger-subtle");
                        this.setTooltip(formatNotSupported);
                    } else {
                        this.setStyle("-fx-background-color: -color-bg-default");
                    }
                }
            }
        });
    }

    @Override public void makeTranslations() {
        // Title
        titleLabel.setText(translator.translate(titleLabel.getText()));

        // Table Columns
        for (var column : trackList.getColumns()) {
            column.setText(translator.translate(column.getText()));
        }

        // Tooltip
        this.formatNotSupported.setText(
            this.translator.translate(this.formatNotSupported.getText())
        );
    }

    public void saveLibrary() {
        // Make in a proper use case
        var libraryManager = new LibraryRepository(Musicx.getInstance().getApp().getSessionFactory());
        libraryManager.save(this.props.library());
    }

    public void tableClick(MouseEvent mouseEvent) {
        // Double click handling
        if (2 == mouseEvent.getClickCount()) {
            var song = this.trackList.getSelectionModel().getSelectedItem();

            this.parent.getAudioPlayerViewController().setCurrentPlayingSong(song);
        }
    }

    @Override public MusicxViewController getParent() {
        return this.parent;
    }

    public TableView<SongViewModel> getTrackList() {
        return this.trackList;
    }
}
