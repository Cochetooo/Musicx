package fr.xahla.musicx.desktop.views.pages.editor.genre;

import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriteria;
import fr.xahla.musicx.desktop.model.entity.Genre;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.domain.service.saveLocalSongs.WriteMetadataToAudioFile;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.control.Button;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.control.TextField;
import javafx.scene.layout.Background;
import javafx.scene.layout.HBox;
import javafx.scene.text.Text;
import javafx.util.StringConverter;
import org.controlsfx.control.ListActionView;
import org.controlsfx.control.textfield.AutoCompletionBinding;
import org.controlsfx.control.textfield.TextFields;

import java.net.URL;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.audioPlayer;
import static fr.xahla.musicx.desktop.context.DesktopContext.scene;
import static fr.xahla.musicx.domain.application.AbstractContext.genreRepository;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

/**
 * View for genre editing.
 * @author Cochetooo
 * @since 0.3.3
 */
public class EditGenre implements Initializable {

    @FXML private Button editButton;

    @FXML private ListActionView<Genre> primaryGenresList;
    @FXML private ListActionView<Genre> secondaryGenresList;
    @FXML private TextField secondaryGenreField;
    @FXML private TextField primaryGenreField;

    private Song song;
    private List<Genre> genreList;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.song = audioPlayer().getEditedSong();

        primaryGenresList.setCellFactory(this::getGenreListFactory);
        secondaryGenresList.setCellFactory(this::getGenreListFactory);

        primaryGenresList.getItems().setAll(song.getPrimaryGenres());
        secondaryGenresList.getItems().setAll(song.getSecondaryGenres());

        genreList = new ArrayList<>();
        final var genreDtoList = genreRepository().findAll();

        genreDtoList.forEach(genreDto -> genreList.add(new Genre(genreDto)));

        TextFields.bindAutoCompletion(primaryGenreField, this::getGenreSuggestionProvider, new GenreStringConverter());
        TextFields.bindAutoCompletion(secondaryGenreField, this::getGenreSuggestionProvider, new GenreStringConverter());
    }

    public void close() {
        scene().getRightNavContent().close();
    }

    @FXML private void proposePrimaryGenre() {
        final var genre = getGenreFromString(primaryGenreField.getText());

        if (null != genre && !primaryGenresList.getItems().contains(genre)) {
            primaryGenresList.getItems().add(genre);
            editButton.setDisable(false);
        }
    }

    @FXML private void proposeSecondaryGenre() {
        final var genre = getGenreFromString(secondaryGenreField.getText());

        if (null != genre && !secondaryGenresList.getItems().contains(genre)) {
            secondaryGenresList.getItems().add(genre);
            editButton.setDisable(false);
        }
    }

    @FXML private void edit() {
        song.setPrimaryGenres(primaryGenresList.getItems());
        song.setSecondaryGenres(secondaryGenresList.getItems());

        songRepository().save(song.getDto());
        new WriteMetadataToAudioFile().execute(song.getDto());

        editButton.setDisable(true);
    }

    // --- Inner classes ---
    static class GenreStringConverter extends StringConverter<Genre> {
        @Override public String toString(final Genre genre) {
            return genre.getName();
        }

        @Override public Genre fromString(final String string) {
            return getGenreFromString(string);
        }
    }

    private List<Genre> getGenreSuggestionProvider(final AutoCompletionBinding.ISuggestionRequest request) {
        final var userText = request.getUserText().toLowerCase();
        return genreList.stream()
            .filter(genre -> genre.getName().toLowerCase().contains(userText))
            .toList();
    }

    private ListCell<Genre> getGenreListFactory(final ListView<Genre> listView) {
        return new ListCell<>() {
            @Override public void updateItem(final Genre genre, final boolean empty) {
                super.updateItem(genre, empty);
                if (empty || null == genre) {
                    setGraphic(null);
                } else {
                    final var text = new Text(genre.getName());
                    final var delete = new Button("X");
                    delete.setBackground(Background.EMPTY);
                    delete.setOnMouseClicked(event -> {
                        listView.getItems().remove(genre);
                        editButton.setDisable(false);
                    });

                    final var hBox = new HBox();
                    hBox.setAlignment(Pos.CENTER_LEFT);
                    hBox.setSpacing(10);
                    hBox.setPadding(new Insets(3, 3, 3, 3));
                    hBox.getChildren().addAll(delete, text);

                    setGraphic(hBox);
                }
            }
        };
    }

    private static Genre getGenreFromString(final String string) {
        final var genresDto = genreRepository().findByCriteria(Map.of(
            GenreSearchCriteria.NAME, string
        ));

        if (genresDto.isEmpty()) {
            return null;
        }

        return new Genre(genresDto.getFirst());
    }
}
