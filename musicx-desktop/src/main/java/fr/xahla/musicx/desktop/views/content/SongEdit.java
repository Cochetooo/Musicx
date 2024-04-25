package fr.xahla.musicx.desktop.views.content;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TextField;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.player;

public class SongEdit implements Initializable {

    @FXML private TextField artistNameField;
    @FXML private TextField albumNameField;
    @FXML private TextField songNameField;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.artistNameField.setText(player().getCurrentSong().getArtist().getName());
        this.albumNameField.setText(player().getCurrentSong().getAlbum().getName());
        this.songNameField.setText(player().getCurrentSong().getTitle());
    }

    @FXML public void edit() {

    }
}
