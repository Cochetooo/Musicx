package fr.xahla.musicx.api.model;

import java.util.List;

public interface LibraryInterface {

    /* @var Long libraryId */

    Long getId();
    LibraryInterface setId(final Long id);

    /* @var List<SongInterface> songs */

    List<? extends SongInterface> getSongs();
    LibraryInterface setSongs(final List<? extends SongInterface> songs);

    /* @var String name */

    String getName();
    LibraryInterface setName(final String name);

    /* @var String folderPath */

    List<String> getFolderPaths();
    LibraryInterface setFolderPaths(final List<String> folderPath);

    /* Hydrate data from another Library */

    LibraryInterface set(final LibraryInterface libraryInterface);

}
