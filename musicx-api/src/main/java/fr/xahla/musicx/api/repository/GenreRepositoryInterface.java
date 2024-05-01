package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;

import java.util.List;
import java.util.Map;

public interface GenreRepositoryInterface {

    List<GenreDto> findByCriteria(final Map<GenreSearchCriterias, Object> criteria);
    List<GenreDto> findAll();

    List<GenreDto> fromSongs(final List<SongDto> songs);

    void create(final GenreDto genre);

}
