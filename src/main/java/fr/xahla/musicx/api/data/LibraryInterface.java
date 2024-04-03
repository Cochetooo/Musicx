package fr.xahla.musicx.api.data;

import java.util.List;

public interface LibraryInterface {

    /* @var Long libraryId */

    Long getId();
    LibraryInterface setId(Long id);

    /* @var List<SongInterface> songs */

    List<? extends SongInterface> getSongs();
    LibraryInterface setSongs(List<? extends SongInterface> songs);

    /* @var String name */

    String getName();
    LibraryInterface setName(String name);

    /* @var String folderPath */

    String getFolderPaths();
    LibraryInterface setFolderPaths(String folderPath);

    /* Hydrate data from another Library */

    LibraryInterface set(LibraryInterface otherLibrary);

}
