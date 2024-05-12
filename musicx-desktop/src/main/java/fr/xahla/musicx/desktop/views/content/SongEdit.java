package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.service.localAudioFile.WriteMetadataToAudioFile;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.TextField;
import javafx.util.Duration;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.player;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

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

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var song = player().getCurrentSong();

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

        this.discTotalField.setText(String.valueOf(song.getAlbum().getDiscTotal()));

        this.trackNoField.setText(String.valueOf(song.getTrackNumber()));
        this.trackNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.trackTotalField.setText(String.valueOf(song.getAlbum().getTrackTotal()));

        this.durationField.setText(DurationHelper.getTimeString(Duration.millis(
            song.getDuration()
        )));

        this.bitRateField.setText(String.valueOf(song.getBitRate()));
        this.sampleRateField.setText(String.valueOf(song.getSampleRate()));

        this.formatField.setText(song.getFormat().name());
        this.filepathField.setText(song.getFilepath());
    }

    @FXML public void change() {
        editButton.setDisable(false);
    }

    @FXML public void edit() {
        final var song = player().getCurrentSong();

        song.setDiscNumber(StringHelper.parseShort(discNoField.getText()));
        song.setTitle(songNameField.getText());
        song.setTrackNumber(StringHelper.parseShort(trackNoField.getText()));

        songRepository().save(song.getDto());
        new WriteMetadataToAudioFile().execute(song.getDto());

        editButton.setDisable(true);
    }

    @FXML public void editArtist() {

    }

    @FXML public void editAlbum() {

    }
}
