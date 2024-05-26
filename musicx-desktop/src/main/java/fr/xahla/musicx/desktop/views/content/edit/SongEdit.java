package fr.xahla.musicx.desktop.views.content.edit;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.service.saveLocalSongs.WriteMetadataToAudioFile;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.TextField;
import javafx.util.Duration;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.desktop.helper.DurationHelper.getTimeString;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * View for song editing.
 * @author Cochetooo
 * @since 0.3.1
 */
public class SongEdit implements Initializable {

    @FXML private Button editButton;

    @FXML private TextField songNameField;

    @FXML private TextField discNoField;
    @FXML private TextField discTotalField;

    @FXML private TextField trackNoField;
    @FXML private TextField trackTotalField;

    @FXML private TextField durationField;
    @FXML private TextField bitRateField;
    @FXML private TextField sampleRateField;
    @FXML private TextField formatField;

    @FXML private TextField filepathField;
    @FXML private TextField createdAtField;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        final var song = audioPlayer().getEditedSong();

        this.songNameField.setText(song.getTitle());
        this.songNameField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.discNoField.setText(String.valueOf(song.getDiscNumber()));
        this.discNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.discTotalField.setText(String.valueOf(
            (null == song.getAlbum())
            ? 1
            : song.getAlbum().getDiscTotal()
        ));

        this.trackNoField.setText(String.valueOf(song.getTrackNumber()));
        this.trackNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.trackTotalField.setText(String.valueOf(
            (null == song.getAlbum())
                ? 1
                : song.getAlbum().getTrackTotal()
        ));

        this.durationField.setText(getTimeString(Duration.millis(
            song.getDuration()
        )));

        this.bitRateField.setText(String.valueOf(song.getBitRate()));
        this.sampleRateField.setText(String.valueOf(song.getSampleRate()));

        this.formatField.setText(song.getFormat().name());
        this.filepathField.setText(song.getFilepath());
        this.createdAtField.setText(
            fr.xahla.musicx.domain.helper.DurationHelper.getDateTimeString(song.getCreatedAt()));
    }

    @FXML public void change() {
        editButton.setDisable(false);
    }

    @FXML public void edit() {
        final var song = audioPlayer().getEditedSong();

        song.setDiscNumber(StringHelper.str_parse_short_safe(discNoField.getText()));
        song.setTitle(songNameField.getText());
        song.setTrackNumber(StringHelper.str_parse_short_safe(trackNoField.getText()));

        songRepository().save(song.getDto());
        new WriteMetadataToAudioFile().execute(song.getDto());

        editButton.setDisable(true);
    }

    @FXML public void editArtist() {

    }

    @FXML public void editAlbum() {
        scene().getRightNavContent().switchContent(FxmlComponent.EDIT_ALBUM, resourceBundle);
    }

    @FXML private void editGenres() {
        scene().getRightNavContent().switchContent(FxmlComponent.EDIT_GENRE, resourceBundle);
    }

    public void close() {
        scene().getRightNavContent().close();
    }
}
