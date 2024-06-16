package fr.xahla.musicx.desktop.views.pages.editor.artist;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.desktop.model.entity.Album;
import fr.xahla.musicx.desktop.model.entity.Artist;
import javafx.beans.property.SimpleIntegerProperty;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.ButtonType;
import javafx.scene.control.SelectionMode;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;

import java.net.URL;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.ResourceBundle;

import static fr.xahla.musicx.domain.application.AbstractContext.artistRepository;
import static fr.xahla.musicx.domain.helper.StringHelper.str_contains_ignore_case;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public class ListArtist implements Initializable {

    @FXML private CustomTextField searchTextField;
    @FXML private TableView<Artist> artistTableView;

    @FXML private TableColumn<Artist, Number> nbSongsColumn;
    @FXML private TableColumn<Artist, Number> nbAlbumsColumn;

    private FilteredList<Artist> artistList;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        artistTableView.getSelectionModel().setSelectionMode(SelectionMode.MULTIPLE);

        this.updateTable();

        nbSongsColumn.setCellValueFactory(this::setNbSongsColumnCellValueFactory);
        nbAlbumsColumn.setCellValueFactory(this::setNbAlbumsColumnCellValueFactory);

        searchTextField.textProperty().addListener((observable, oldValue, newValue) -> onSearch(newValue));
    }

    /**
     * Retrieve all the artists from the repository and sort them.
     */
    private void updateTable() {
        final var artistDtoList = artistRepository().findAll();

        artistList = new FilteredList<>(FXCollections.observableList(new ArrayList<>(
            artistDtoList.stream()
                .map(Artist::new)
                .toList()
        )));

        artistTableView.setItems(artistList);
    }

    private void onSearch(final String text) {
        artistList.setPredicate(artist -> str_contains_ignore_case(artist.getName(), text));
    }

    /**
     * Switch to create artist page.
     */
    @FXML private void createArtist() {

    }

    /**
     * Merge an artist to another artist.
     */
    @FXML private void mergeArtist() {


        //this.updateTable();
    }

    /**
     * Delete an artist
     */
    @FXML private void deleteArtist() {
        final var confirmationResponse = FxmlHelper.showConfirmAlert(
            "Delete Artist",
            "Are you sure you want to delete this artist?"
        );

        if (confirmationResponse.isEmpty()) {
            return;
        }

        if (ButtonType.NO == confirmationResponse.get() || ButtonType.CANCEL == confirmationResponse.get()) {
            return;
        }

        artistRepository().delete(
            artistTableView.getSelectionModel().getSelectedItem().getDto(),
            false
        );

        this.updateTable();
    }

    /* Table Cell Factories */
    private ObservableValue<Number> setNbSongsColumnCellValueFactory(TableColumn.CellDataFeatures<Artist, Number> cellData) {
        return new SimpleIntegerProperty(artistRepository().getSongs(cellData.getValue().getDto()).size());
    }

    private ObservableValue<Number> setNbAlbumsColumnCellValueFactory(TableColumn.CellDataFeatures<Artist, Number> cellData) {
        return new SimpleIntegerProperty(artistRepository().getAlbums(cellData.getValue().getDto()).size());
    }


}
