package fr.xahla.musicx.desktop.views._content.edit;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.interfaces.EditFormInterface;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.service.save.EditAlbumService;
import fr.xahla.musicx.domain.helper.StringHelper;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.*;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.layout.VBox;
import javafx.stage.FileChooser;

import java.net.URL;
import java.util.ArrayList;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.artistRepository;

/**
 * View for album editing.
 * @author Cochetooo
 * @since 0.3.1
 */
public class AlbumEdit implements Initializable, EditFormInterface {

    @FXML private VBox albumEditContainer;

    @FXML private Button editButton;

    @FXML private Label artworkDimensionLabel;
    @FXML private TextField albumNameField;

    @FXML private TextField catalogNoField;
    @FXML private TextField discTotalField;
    @FXML private TextField trackTotalField;

    @FXML private ComboBox<ReleaseType> albumTypeComboBox;
    @FXML private DatePicker releaseDatePicker;

    @FXML private TextField artworkUrlField;
    @FXML private ImageView artworkView;

    private Album album;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.setupListeners();

        final var editedSong = audioPlayer().getEditedSong();
        this.album = editedSong.getAlbum();

        if (null != editedSong.getArtist()) {
            this.addArtistAlbumsComboBox();
        }

        if (null == album) {

            return;
        }

        this.updateFields();
    }

    @FXML private void editArtist() {

    }

    @FXML private void editLabel() {

    }

    @Override public void applyChange() {
        editButton.setDisable(false);
    }

    @FXML private void chooseArtwork() {
        final var fileChooser = new FileChooser();
        final var imageFilter = new FileChooser.ExtensionFilter("Images",
            "*.jpg", "*.jpeg", "*.png", "*.gif"
        );
        fileChooser.getExtensionFilters().add(imageFilter);

        final var selectedFile = fileChooser.showOpenDialog(null);

        if (null != selectedFile) {
            this.artworkUrlField.setText(selectedFile.getAbsolutePath());
            this.artworkView.setImage(new Image(artworkUrlField.getText()));
            this.applyChange();
        }
    }

    @Override @FXML public void edit() {
        if (null == album) {
            album = new Album(AlbumDto.builder().build());
        }

        album.setArtworkUrl(artworkUrlField.getText());
        album.setCatalogNo(catalogNoField.getText());
        album.setDiscTotal(StringHelper.str_parse_short_safe(discTotalField.getText()));
        album.setName(albumNameField.getText());
        album.setReleaseDate(releaseDatePicker.getValue());
        album.setTrackTotal(StringHelper.str_parse_short_safe(trackTotalField.getText()));
        album.setType(albumTypeComboBox.getValue());

        new EditAlbumService().execute(album, ()
            -> this.editButton.setDisable(true));
    }

    @FXML private void close() {
        scene().getNavContent().close();
    }

    private void addArtistAlbumsComboBox() {
        final var editedSong = audioPlayer().getEditedSong();

        final var artistAlbumComboBox = new ComboBox<Album>();
        final var artistAlbums = new SimpleListProperty<Album>(FXCollections.observableList(new ArrayList<>()));

        artistRepository().getAlbums(editedSong.getArtist().getDto()).forEach(albumDto
            -> artistAlbums.add(new Album(albumDto)));

        artistAlbumComboBox.setItems(artistAlbums);
        artistAlbumComboBox.setCellFactory(param -> new ListCell<>(){
            @Override protected void updateItem(final Album item, final boolean empty) {
                super.updateItem(item, empty);

                if (empty || null == item) {
                    setText(null);
                } else {
                    setText(item.getName());
                }
            }
        });
        artistAlbumComboBox.getSelectionModel().selectedItemProperty().addListener(((observable, oldValue, newValue) -> {
            if (null == newValue || oldValue == newValue) {
                return;
            }

            album = newValue;
            this.updateFields();
        }));

        albumEditContainer.getChildren().add(1, artistAlbumComboBox);
    }

    @Override public void setupListeners() {
        this.albumNameField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                applyChange();
            }
        });

        this.catalogNoField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                applyChange();
            }
        });

        this.discTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                applyChange();
            }
        });

        this.trackTotalField.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!oldValue.equals(newValue)) {
                applyChange();
            }
        });

        this.albumTypeComboBox.getItems().addAll(ReleaseType.values());
        this.albumTypeComboBox.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                applyChange();
            }
        });

        this.releaseDatePicker.valueProperty().addListener((observable, oldValue, newValue) -> {
            if (oldValue != newValue) {
                applyChange();
            }
        });

        this.artworkDimensionLabel.setTextFill(ColorHelper.GRAY);
        this.artworkView.imageProperty().addListener((observable, oldValue, newValue)
            -> this.artworkDimensionLabel.setText(newValue.getWidth() + " x " + newValue.getHeight()));
    }

    @Override public void updateFields() {
        this.albumNameField.setText(album.getName());
        this.catalogNoField.setText(album.getCatalogNo());
        this.discTotalField.setText(String.valueOf(album.getDiscTotal()));
        this.trackTotalField.setText(String.valueOf(album.getTrackTotal()));
        this.albumTypeComboBox.setValue(album.getType());
        this.releaseDatePicker.setValue(album.getReleaseDate());
        this.artworkUrlField.setText(album.getArtworkUrl());

        if (null != album.getArtworkUrl() && !album.getArtworkUrl().isEmpty()) {
            this.artworkView.setImage(new Image(album.getArtworkUrl()));
        }
    }
}
