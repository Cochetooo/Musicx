package fr.xahla.musicx.domain.helper;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.URISyntaxException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public final class JsonHelper {

    public static JSONObject loadJsonFromResource(final Class<?> caller, final String resourceName) {
        try {
            return loadJsonFromFile(
                Objects.requireNonNull(caller.getResource(resourceName)).toURI().toString()
            );
        }  catch (URISyntaxException exception) {
            logger().log(Level.SEVERE, resourceName + " location is not a valid URI.", exception);
            return new JSONObject();
        }
    }

    public static JSONObject loadJsonFromFile(final String filename) {
        try {
            final var jsonContent = new String(Files.readAllBytes(Paths.get(
                filename
            )));

            return new JSONObject(jsonContent);
        } catch (final IOException exception) {
            logger().log(Level.SEVERE, filename + " is not found or unable to be read.", exception);
            return new JSONObject();
        } catch (final JSONException exception) {
            logger().log(Level.SEVERE, filename + " is not a valid JSON file", exception);
            return new JSONObject();
        }
    }

    public static void saveJsonToFile(final String filename, final JSONObject jsonObject) {
        try {
            Files.writeString(Paths.get(filename), jsonObject.toString());
        } catch (final IOException exception) {
            logger().log(Level.SEVERE, "Cannot save file " + filename, exception);
        }
    }

}
