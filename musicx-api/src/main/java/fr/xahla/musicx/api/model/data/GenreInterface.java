package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.GenreDto;

import java.util.List;

public interface GenreInterface {

    /* --- DTO Cast --- */

    GenreInterface fromDto(final GenreDto albumDto);
    GenreDto toDto();

    /* --- PRIMARY KEY --- */

    Long getId();
    GenreInterface setId(final Long id);

    /* --- Name --- */

    String getName();
    GenreInterface setName(final String name);

    /* --- Parents --- */

    List<? extends GenreInterface> getParents();
    GenreInterface setParents(final List<? extends GenreInterface> parents);

}
