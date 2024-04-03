package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.api.data.LibraryInterface;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

public class LibraryViewModel implements ViewModelInterface, LibraryInterface {

    private final LongProperty id;
    private final ListProperty<SongViewModel> songs;
    private final StringProperty folderPaths;
    private final StringProperty name;

    public LibraryViewModel() {
        this.id = new SimpleLongProperty();
        this.songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.folderPaths = new SimpleStringProperty();
        this.name = new SimpleStringProperty();
    }

    private ObservableList<SongViewModel> convertFromSongs(List<? extends SongInterface> songs) {
        var songViewModels = FXCollections.observableList(new ArrayList<SongViewModel>());
        songs.forEach((song) -> songViewModels.add(new SongViewModel().set(song)));
        return songViewModels;
    }

    @Override public LibraryViewModel set(LibraryInterface library) {
        if (null != library.getId()) {
            this.setId(library.getId());
        }

        if (null != library.getSongs()) {
            this.setSongs(library.getSongs());
        }

        if (null != library.getFolderPaths()) {
            this.setFolderPaths(library.getFolderPaths());
        }

        if (null != library.getName()) {
            this.setName(library.getName());
        }

        return this;
    }

    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public LibraryViewModel setId(Long id) {
        this.id.set(id);
        return this;
    }

    public ObservableList<SongViewModel> getSongs() {
        return songs.get();
    }

    public ListProperty<SongViewModel> songsProperty() {
        return songs;
    }

    public LibraryViewModel setSongs(List<? extends SongInterface> songs) {
        this.getSongs().setAll(this.convertFromSongs(songs));
        return this;
    }

    public String getFolderPaths() {
        return folderPaths.get();
    }

    public StringProperty folderPathsProperty() {
        return folderPaths;
    }

    public LibraryViewModel setFolderPaths(String folderPaths) {
        this.folderPaths.set(folderPaths);
        return this;
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public LibraryViewModel setName(String name) {
        this.name.set(name);
        return this;
    }
}
