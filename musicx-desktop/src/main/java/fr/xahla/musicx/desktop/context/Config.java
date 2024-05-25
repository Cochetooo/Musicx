package fr.xahla.musicx.desktop.context;

import org.json.JSONObject;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.helper.JsonHelper.json_load_from_file;
import static fr.xahla.musicx.domain.helper.StringHelper.str_is_null_or_blank;

/**
 * User configurations such as settings or active scenes data.<br>
 * Handling with Json file <code>config.json</code>.
 * @author Cochetooo
 * @since 0.3.3
 */
public class Config {

    private final JSONObject configJson;
    private final String configPath;

    public Config() {
        configPath = env("CONFIG_URL");
        configJson = json_load_from_file(configPath);
    }

    public String getActiveScene() {
        return configJson.getString("activeScene");
    }

    public void setActiveScene(final String activeScene) {
        configJson.put("activeScene", activeScene);
        save();
    }

    public String getLanguage() {
        return configJson.getString("language");
    }

    public void setLanguage(final String language) {
        configJson.put("language", language);
        save();
    }

    public List<String> getLocalFolders() {
        if (str_is_null_or_blank(configJson.getString("localFolders"))) {
            return new ArrayList<>();
        }

        return Arrays.asList(configJson.getString("localFolders").split(";"));
    }

    public void setLocalFolders(final List<String> localFolders) {
        configJson.put("localFolders", String.join(";", localFolders));
        save();
    }

    public Object getSetting(final String key) {
        return configJson.getJSONObject("settings").get(key);
    }

    public void setSetting(final String key, final Object value) {
        configJson.getJSONObject("settings").put(key, value);
        save();
    }

    private void save() {
        try {
            Files.writeString(Path.of(configPath), configJson.toString(4));
        } catch (IOException exception) {
            logger().error(exception, "IO_FILE_SAVE_ERROR", configPath);
        }
    }

}
