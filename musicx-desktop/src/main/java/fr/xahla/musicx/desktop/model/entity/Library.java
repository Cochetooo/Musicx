package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.LibraryInterface;
import fr.xahla.musicx.api.model.SongInterface;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

/** <b>Class that defines the Library Model for desktop view usage.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class Library implements LibraryInterface {

    private final LongProperty id;
    private final ListProperty<Song> songs;
    private final StringProperty name;
    private final ListProperty<String> folderPaths;

    public Library() {
        this.id = new SimpleLongProperty();
        this.songs = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
        this.name = new SimpleStringProperty();
        this.folderPaths = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));
    }

    private ObservableList<Song> convertFromSongs(List<? extends SongInterface> songs) {
        var songViewModels = FXCollections.observableList(new ArrayList<Song>());
        songs.forEach((song) -> songViewModels.add(new Song().set(song)));
        return songViewModels;
    }

    @Override public Library set(LibraryInterface library) {
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

    public Library setId(Long id) {
        this.id.set(id);
        return this;
    }

    public ObservableList<Song> getSongs() {
        return songs.get();
    }

    public ListProperty<Song> songsProperty() {
        return songs;
    }

    public Library setSongs(List<? extends SongInterface> songs) {
        this.getSongs().setAll(this.convertFromSongs(songs));
        return this;
    }

    public ObservableList<String> getFolderPaths() {
        return folderPaths.get();
    }

    public ListProperty<String> folderPathsProperty() {
        return folderPaths;
    }

    public Library setFolderPaths(List<String> folderPaths) {
        this.getFolderPaths().setAll(folderPaths);
        return this;
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Library setName(String name) {
        this.name.set(name);
        return this;
    }

}
