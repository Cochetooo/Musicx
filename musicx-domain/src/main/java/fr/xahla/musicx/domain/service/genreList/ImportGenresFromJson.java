package fr.xahla.musicx.domain.service.genreList;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriteria;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.helper.JsonHelper;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Logger;

import org.json.JSONObject;

import static fr.xahla.musicx.domain.application.AbstractContext.*;

/**
 * Read a JSON File and import all data as genres into the database.
 * @author Cochetooo
 * @since 0.3.0
 */
public final class ImportGenresFromJson {

    private int nbCreated;

    private JSONObject jsonContent;

    /**
     * @since 0.3.0
     */
    public void execute(final String jsonPath) {
        jsonContent = JsonHelper.json_load_from_file(jsonPath);

        for (final var entry : jsonContent.toMap().entrySet()) {
            if (entry.getValue() instanceof final List<?> array) {
                saveGenre(entry.getKey(), array.stream().map(Object::toString).toList());
            }
        }
    }

    private Long saveGenre(final String name, final List<String> parents) {
        final var parentIds = new ArrayList<Long>();

        if (!genreRepository().findByCriteria(Map.of(
            GenreSearchCriteria.NAME, name
        )).isEmpty()) {
            return 0L;
        }

        if (!parents.isEmpty()) {
            parents.forEach(parentName -> {
                final var exists = genreRepository().findByCriteria(Map.of(
                    GenreSearchCriteria.NAME, parentName
                ));

                if (!exists.isEmpty()) {
                    parentIds.add(exists.getFirst().getId());
                } else {
                    final var parentGenre = jsonContent.getJSONArray(parentName).toList().stream()
                        .map(Object::toString).toList();

                    parentIds.add(saveGenre(parentName, parentGenre));
                }
            });
        }

        final var genreDto = GenreDto.builder()
            .name(name)
            .parentIds(parentIds)
            .build();

        genreRepository().save(genreDto);

        nbCreated++;

        return genreDto.getId();
    }

    /* -------------- Executable -------------- */

    public static void main(final String[] args) {
        new Context();

        final var service = new ImportGenresFromJson();
        service.execute(env("GENRES_URL"));

        logger().info("Nb created: " + service.nbCreated);
    }

    static class Context extends AbstractContext {
        protected Context() {
            super(Logger.getLogger(Context.class.getName()));
        }
    }

}
