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

    @FXML private Button editButton;

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
        final var album = player().getEditedSong().getAlbum();

        this.albumNameField.setText(album.getName());
        this.albumNameField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.catalogNoField.setText(album.getCatalogNo());
        this.catalogNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.discTotalField.setText(String.valueOf(album.getDiscTotal()));
        this.discTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.trackTotalField.setText(String.valueOf(album.getTrackTotal()));
        this.trackTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                change();
            }
        });

        this.albumTypeComboBox.getItems().addAll(AlbumType.values());
        this.albumTypeComboBox.setValue(album.getType());
        this.albumTypeComboBox.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                change();
            }
        });

        this.releaseDatePicker.setValue(album.getReleaseDate());
        this.releaseDatePicker.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                change();
            }
        });

        this.artworkUrlField.setText(album.getArtworkUrl());

        this.artworkDimensionLabel.setTextFill(ColorHelper.GRAY);

        this.artworkView.imageProperty().addListener((observable, oldValue, newValue)
            -> this.artworkDimensionLabel.setText(newValue.getWidth() + " x " + newValue.getHeight()));

        if (null != album.getArtworkUrl() && !album.getArtworkUrl().isEmpty()) {
            this.artworkView.setImage(new Image(album.getArtworkUrl()));
        }
    }

    @FXML public void editArtist() {

    }

    @FXML public void editLabel() {

    }

    @FXML public void change() {
        editButton.setDisable(false);
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
            this.change();
        }
    }

    @FXML public void edit() {
        final var album = player().getEditedSong().getAlbum();

        album.setArtworkUrl(artworkUrlField.getText());
        album.setCatalogNo(catalogNoField.getText());
        album.setDiscTotal(StringHelper.parseShort(discTotalField.getText()));
        album.setName(albumNameField.getText());
        album.setReleaseDate(releaseDatePicker.getValue());
        album.setTrackTotal(StringHelper.parseShort(trackTotalField.getText()));
        album.setType(albumTypeComboBox.getValue());

        albumRepository().save(album.getDto());
        new WriteMetadataToAudioFile().execute(album.getDto());

        this.editButton.setDisable(true);
    }
}
