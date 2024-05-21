package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.model.enums.SceneTab;
import org.json.JSONObject;

import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.helper.JsonHelper.json_load_from_file;

/**
 * User configurations such as settings or active scenes data.<br>
 * Handling with Json file <code>config.json</code>.
 * @author Cochetooo
 * @since 0.3.3
 */
public class Config {

    private final JSONObject configJson;

    public Config() {
        configJson = json_load_from_file(env("CONFIG_URL"));
    }

    public SceneTab getActiveScene() {
        return SceneTab.valueOf(configJson.getString("activeScene"));
    }

    public void setActiveScene(final SceneTab activeScene) {
        configJson.put("activeScene", activeScene.name());
    }

    public List<String> getLocalFolders() {
        return Arrays.asList(configJson.getString("localFolders").split(";"));
    }

    public void setLocalFolders(final List<String> localFolders) {
        configJson.put("localFolders", localFolders.stream()
            .collect(Collectors.joining(";")));
    }

    public Object getSetting(final String key) {
        return configJson.getJSONObject("settings").get(key);
    }

    public void setSetting(final String key, final Object value) {
        configJson.getJSONObject("settings").put(key, value);
    }

}
