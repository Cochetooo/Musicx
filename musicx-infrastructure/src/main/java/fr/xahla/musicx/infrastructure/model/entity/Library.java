package fr.xahla.musicx.infrastructure.model.entity;

import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.infrastructure.model.data.LibraryInterface;
import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.List;

/** <b>Class that defines the Library Model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
@Entity
@Table(name="libraries")
public class Library implements LibraryInterface {
    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    @Column(name="library_id")
    private Long id;

    @ManyToMany
    @JoinTable(
        name="Library_Song",
        joinColumns={@JoinColumn(name="library_id")},
        inverseJoinColumns={@JoinColumn(name="song_id")}
    )
    private List<Song> songs;

    private String name;

    @ElementCollection(fetch=FetchType.EAGER)
    private List<String> folderPaths;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    public Long getId() {
        return id;
    }

    public Library setId(final Long id) {
        this.id = id;
        return this;
    }

    public List<Song> getSongs() {
        return songs;
    }

    public Library setSongs(final List<? extends SongInterface> songs) {
        var songEntities = new ArrayList<Song>();

        songs.forEach((song) -> songEntities.add(new Song().set(song)));

        this.songs = songEntities;
        return this;
    }

    public String getName() {
        return name;
    }

    public Library setName(final String name) {
        this.name = name;
        return this;
    }

    public List<String> getFolderPaths() {
        return folderPaths;
    }

    public Library setFolderPaths(final List<String> folderPaths) {
        this.folderPaths = folderPaths;
        return this;
    }

    @Override public Library set(final LibraryInterface otherLibrary) {
        if (null != otherLibrary.getId() && 0 != otherLibrary.getId()) {
            this.setId(otherLibrary.getId());
        }

        if (null != otherLibrary.getName()) {
            this.setName(otherLibrary.getName());
        }

        if (null != otherLibrary.getFolderPaths()) {
            this.setFolderPaths(otherLibrary.getFolderPaths());
        }

        if (null != otherLibrary.getSongs()) {
            this.setSongs(otherLibrary.getSongs());
        }

        return this;
    }
}
