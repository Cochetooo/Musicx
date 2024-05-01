package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;

import java.util.List;

public interface GenreListManagerInterface {

    int PRIMARY_GENRE = 0x0;
    int SECONDARY_GENRE = 0x1;
    int BOTH_GENRES = 0x2;

    List<? extends GenreDto> get();
    GenreRepositoryInterface getRepository();

    void set(final List<? extends GenreDto> genres);

}
