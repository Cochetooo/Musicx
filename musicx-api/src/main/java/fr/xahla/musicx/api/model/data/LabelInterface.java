package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.LabelDto;

import java.util.List;

public interface LabelInterface {

    /* --- DTO Cast --- */

    LabelInterface fromDto(final LabelDto labelDto);
    LabelDto toDto();

    /* --- PRIMARY KEY --- */

    Long getId();
    LabelInterface setId(final Long id);

    /* --- Genres --- */

    List<? extends GenreInterface> getGenres();
    LabelInterface setGenres(final List<? extends GenreInterface> genres);

    /* --- Name --- */

    String getName();
    LabelInterface setName(final String name);

    /* --- Releases --- */

    List<? extends AlbumInterface> getReleases();
    LabelInterface setReleases(final List<? extends AlbumInterface> releases);

}
