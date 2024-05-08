package fr.xahla.musicx.domain.model.data;

import fr.xahla.musicx.api.model.SongDto;

import java.util.List;

/** <b>Interface for Library Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface LibraryInterface {

    /* @var Long libraryId */

    Long getId();
    LibraryInterface setId(final Long id);

    /* @var List<SongInterface> songs */

    List<? extends SongDto> getSongs();
    LibraryInterface setSongs(final List<? extends SongDto> songs);

    /* @var String name */

    String getName();
    LibraryInterface setName(final String name);

    /* @var String folderPath */

    List<String> getFolderPaths();
    LibraryInterface setFolderPaths(final List<String> folderPath);

    /* Hydrate data from another Library */

    LibraryInterface set(final LibraryInterface libraryInterface);

}
