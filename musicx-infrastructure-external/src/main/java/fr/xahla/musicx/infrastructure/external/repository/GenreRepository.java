package fr.xahla.musicx.infrastructure.external.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;
import fr.xahla.musicx.domain.helper.JsonHelper;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class GenreRepository implements GenreRepositoryInterface {

    private final JSONObject genreJson;
    private final String jsonUrl;

    private final List<GenreDto> genreListCache;

    public GenreRepository() {
        this.jsonUrl = env("GENRES_URL");

        this.genreJson = JsonHelper.loadJsonFromFile(
            this.jsonUrl
        );

        this.genreListCache = this.findAll_init();
    }

    @Override public List<GenreDto> findByCriteria(final Map<GenreSearchCriterias, Object> criterias) {
        var result = this.findAll();

        for (final var criteria : criterias.entrySet()) {
            switch (criteria.getKey()) {
                case NAME -> result = result.stream().filter((genre) ->
                    genre.getName().equalsIgnoreCase(criteria.getValue().toString())
                ).toList();
            }
        }

        return result;
    }

    @Override public List<GenreDto> findAll() {
        return new ArrayList<>(this.genreListCache);
    }

    private List<GenreDto> findAll_init() {
        return genreJson.toMap().entrySet().stream().map((entry) -> {
            final var genreName = entry.getKey();

            final var entryValue = entry.getValue();

            if (entryValue instanceof String[] parentList) {
                final var parentGenres = Arrays.stream(parentList).map(
                    (parent) -> this.findByCriteria(Map.of(GenreSearchCriterias.NAME, genreName)).getFirst()
                ).toList();

                return new GenreDto().setName(genreName).setParents(parentGenres);
            } else {
                logger().severe("Invalid genre value in JSON file for name " + genreName + ": " + entryValue);
                return null;
            }
        }).toList();
    }

    @Override public List<GenreDto> fromSongs(final List<SongDto> songs) {
        return songs.stream()
            .flatMap(song -> song.getPrimaryGenres().stream())
            .distinct()
            .toList();
    }

    @Override public void create(final GenreDto genre) {
        if (this.genreListCache.contains(genre)) {
            logger().warning("Genre " + genre.getName() + " already exists");
            return;
        }

        this.genreListCache.add(genre);

        this.genreJson.put(
            genre.getName(),
            genre.getParents().stream().map(GenreDto::getName).toArray()
        );

        JsonHelper.saveJsonToFile(
            this.jsonUrl,
            this.genreJson
        );
    }
}
