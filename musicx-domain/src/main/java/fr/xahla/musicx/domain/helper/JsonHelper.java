package fr.xahla.musicx.domain.helper;

import fr.xahla.musicx.domain.logging.LogMessage;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.URISyntaxException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Objects;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Utility class for Json library
 * @author Cochetooo
 */
public final class JsonHelper {

    /**
     * @param caller In order to find the resource file, the caller class will be called.
     * @param resourceName The path from the resource starting from the caller class package but in the <i>resources</i> folder.
     * @return A JSONObject from the content of the file, if not found, an empty JSONObject.
     */
    public static JSONObject json_load_from_resource(final Class<?> caller, final String resourceName) {
        try {
            return json_load_from_file(
                Objects.requireNonNull(caller.getResource(resourceName)).toURI().toString()
            );
        }  catch (URISyntaxException exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_IO_NOT_VALID_URI.msg(), resourceName),
                exception
            );

            return new JSONObject();
        }
    }

    /**
     * @param filename The filepath as a String
     * @return A JSONObject from the content of the file, otherwise if not found or not valid, an empty JSONObject.
     */
    public static JSONObject json_load_from_file(final String filename) {
        try {
            final var jsonContent = new String(Files.readAllBytes(Paths.get(
                filename
            )));

            return new JSONObject(jsonContent);
        } catch (final IOException exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_IO_FILE_NOT_FOUND.msg(), filename),
                exception
            );

            return new JSONObject();
        } catch (final JSONException exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_JSON_NOT_VALID.msg(), filename),
                exception
            );

            return new JSONObject();
        }
    }

    /**
     * @param filename The filepath as a String
     * @param jsonObject The JSONObject to write in a file.
     */
    public static void json_save_as_file(final String filename, final JSONObject jsonObject) {
        try {
            Files.writeString(Paths.get(filename), jsonObject.toString());
        } catch (final IOException exception) {
            logger().log(
                Level.SEVERE,
                String.format(LogMessage.ERROR_IO_SAVE.msg(), filename),
                exception
            );
        }
    }

}
