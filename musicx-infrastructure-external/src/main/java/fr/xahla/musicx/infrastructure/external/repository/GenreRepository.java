package fr.xahla.musicx.infrastructure.external.repository;

import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;
import fr.xahla.musicx.domain.helper.JsonHelper;
import fr.xahla.musicx.infrastructure.external.model.Genre;
import org.json.JSONObject;

import java.util.Arrays;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class GenreRepository implements GenreRepositoryInterface {

    private final JSONObject genreJson;

    public GenreRepository() {
        this.genreJson = JsonHelper.loadJsonFromFile(
            env("GENRES_URL")
        );
    }

    @Override public List<Genre> findByCriterias(final GenreSearchCriterias... criterias) {
        return List.of();
    }

    @Override public List<Genre> findAll() {
        return genreJson.toMap().entrySet().stream().map((entry) -> {
            final var genreName = entry.getKey();

            final var entryValue = entry.getValue();

            if (entryValue instanceof String[] parentList) {
                final var parentGenres = Arrays.stream(parentList).map(
                    (parent) -> this.findByCriterias(GenreSearchCriterias.NAME).getFirst()
                ).toList();

                return new Genre()
            } else {
                logger().severe("Invalid genre value in JSON file for name " + genreName + ": " + entryValue);
                return;
            }
        }).collect();
    }

    @Override public List<Genre> fromSongs(final List<SongInterface> songs) {
        return List.of();
    }

    @Override public void save(final GenreInterface genre) {

    }
}
