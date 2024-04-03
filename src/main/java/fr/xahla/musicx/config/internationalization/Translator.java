package fr.xahla.musicx.config.internationalization;

import org.json.JSONArray;
import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;

public class Translator {

    private static Locale locale;

    private final String DEFAULT_PATH = "src/main/resources/fr/xahla/musicx/translations/";

    private final String path;

    private Map<String, String> translations;

    public Translator(
        final String file
    ) {
        if (null == locale) {
            System.out.println("A translator was initialized when locale hasn't been set.");
            locale = new Locale.Builder().setLanguage("en").setRegion("US").build();
        }

        this.path = DEFAULT_PATH + file + "." + locale.getLanguage() + ".json";
        this.translations = new HashMap<>();

        this.setTranslations();
    }

    /**
     * Build the translations HashMap from the path we built in the constructor.
     */
    private void setTranslations() {
        try (var fileReader = new FileReader(this.path)) {
            var jsonTokener = new JSONTokener(fileReader);
            var jsonTranslations = new JSONObject(jsonTokener);
            this.translations = this.convertJsonToHashMap(jsonTranslations);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private Map<String, String> convertJsonToHashMap(JSONObject json) {
        var hashMap = new HashMap<String, String>();

        for (String key : json.keySet()) {
            var value = json.get(key);
            if (value instanceof JSONObject jsonValue) {
                var nestedHashMap = this.convertJsonToHashMap(jsonValue);

                for (String nestedKey : nestedHashMap.keySet()) {
                    hashMap.put(key + "." + nestedKey, nestedHashMap.get(nestedKey));
                }
            } else {
                hashMap.put(key, value.toString());
            }
        }

        return hashMap;
    }

    public String translate(String key) {
        var translation = this.translations.get(key);

        return (null == translation ? key : translation);
    }

    public static void setLocale(Locale pLocale) {
        locale = pLocale;
    }

}
