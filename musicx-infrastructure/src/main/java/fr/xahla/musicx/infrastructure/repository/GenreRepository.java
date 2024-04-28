package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.api.model.GenreInterface;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.URISyntaxException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.List;
import java.util.Objects;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class GenreRepository implements GenreRepositoryInterface {

    public static JSONObject genres;

    static {
        try {
            final var jsonContent = new String(Files.readAllBytes(Paths.get(
                Objects.requireNonNull(GenreRepository.class.getResource("genres.json")).toURI().toString()
            )));

            genres = new JSONObject(jsonContent);
        } catch (final IOException exception) {
            logger().log(Level.SEVERE, "genres.json is not found or unable to be read.", exception);
        } catch (final JSONException exception) {
            logger().log(Level.SEVERE, "genres.json is not a valid JSON file", exception);
        } catch (URISyntaxException exception) {
            logger().log(Level.SEVERE, "genres.json location is not a valid URI.", exception);
        }
    }

    @Override public GenreInterface findByName(final String name) {
        if (!genres.has(name)) {
            logger().info("Genre " + name + " not found");
            return null;
        }

        final var genre = genres.getJSONArray(name).toList();

        logger().info("Found: " + name + " (" + genre.stream().map(Object::toString).collect(Collectors.joining(", ")) + ")");
        return null;
    }

    @Override public List<GenreInterface> findAll() {
        return List.of();
    }

}
