package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.GenreInterface;

import java.util.List;

public interface GenreRepositoryInterface {

    GenreInterface findByName(final String name);
    List<GenreInterface> findAll();

}
