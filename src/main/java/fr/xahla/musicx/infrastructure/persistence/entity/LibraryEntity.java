package fr.xahla.musicx.infrastructure.persistence.entity;

import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.api.data.LibraryInterface;
import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Entity
@Table(name="libraries")
public class LibraryEntity implements LibraryInterface {
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
    private List<SongEntity> songs;

    private String name;

    private String folderPath;

    // *********************** //
    // *  GETTERS / SETTERS  * //
    // *********************** //

    public Long getId() {
        return id;
    }

    public LibraryEntity setId(Long id) {
        this.id = id;
        return this;
    }

    public List<SongEntity> getSongs() {
        return songs;
    }

    public LibraryEntity setSongs(List<? extends SongInterface> songs) {
        var songEntities = new ArrayList<SongEntity>();

        songs.forEach((song) -> songEntities.add(new SongEntity().set(song)));

        this.songs = songEntities;
        return this;
    }

    public String getName() {
        return name;
    }

    public LibraryEntity setName(String name) {
        this.name = name;
        return this;
    }

    public String getFolderPaths() {
        return folderPath;
    }

    public LibraryEntity setFolderPaths(String folderPath) {
        this.folderPath = folderPath;
        return this;
    }

    @Override public LibraryEntity set(LibraryInterface otherLibrary) {
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
