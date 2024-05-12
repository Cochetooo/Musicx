package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.api.model.enums.AlbumType;
import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.service.localAudioFile.WriteMetadataToAudioFile;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.paint.Color;
import javafx.stage.FileChooser;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.DesktopContext.player;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;

public class AlbumEdit implements Initializable {

    @FXML private Label artworkDimensionLabel;
    @FXML private TextField albumNameField;

    @FXML private TextField catalogNoField;
    @FXML private TextField discTotalField;
    @FXML private TextField trackTotalField;

    @FXML private ComboBox<AlbumType> albumTypeComboBox;
    @FXML private DatePicker releaseDatePicker;

    @FXML private TextField artworkUrlField;
    @FXML private ImageView artworkView;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var album = player().getCurrentSong().getAlbum();

        this.albumNameField.setText(album.getName());

        this.catalogNoField.setText(album.getCatalogNo());
        this.discTotalField.setText(String.valueOf(album.getDiscTotal()));
        this.trackTotalField.setText(String.valueOf(album.getTrackTotal()));

        this.albumTypeComboBox.getItems().addAll(AlbumType.values());
        this.albumTypeComboBox.setValue(album.getType());

        this.releaseDatePicker.setValue(album.getReleaseDate());

        this.artworkUrlField.setText(album.getArtworkUrl());

        this.artworkDimensionLabel.setTextFill(ColorHelper.GRAY);

        this.artworkView.imageProperty().addListener((observable, oldValue, newValue)
            -> this.artworkDimensionLabel.setText(newValue.getWidth() + " x " + newValue.getHeight()));

        this.artworkView.setImage(new Image(album.getArtworkUrl()));
    }

    @FXML public void editArtist() {

    }

    @FXML public void editLabel() {

    }

    @FXML public void chooseArtwork() {
        final var fileChooser = new FileChooser();
        final var imageFilter = new FileChooser.ExtensionFilter("Images",
            "*.jpg", "*.jpeg", "*.png", "*.gif"
        );
        fileChooser.getExtensionFilters().add(imageFilter);

        final var selectedFile = fileChooser.showOpenDialog(null);

        if (null != selectedFile) {
            this.artworkUrlField.setText(selectedFile.getAbsolutePath());
            this.artworkView.setImage(new Image(artworkUrlField.getText()));
        }
    }

    @FXML public void edit() {
        final var album = player().getCurrentSong().getAlbum();

        album.setArtworkUrl(artworkUrlField.getText());
        album.setCatalogNo(catalogNoField.getText());
        album.setDiscTotal(StringHelper.parseShort(discTotalField.getText()));
        album.setName(albumNameField.getText());
        album.setReleaseDate(releaseDatePicker.getValue());
        album.setTrackTotal(StringHelper.parseShort(trackTotalField.getText()));
        album.setType(albumTypeComboBox.getValue());

        albumRepository().save(album.getDto());
        new WriteMetadataToAudioFile().execute(album.getDto());
    }
}
