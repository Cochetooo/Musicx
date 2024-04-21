package fr.xahla.musicx.core.service;

import fr.xahla.musicx.api.model.SongInterface;
import org.json.JSONObject;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class GetArtworkFromLastFm {

    private static final String API_KEY = "f17c4b2c645d912c0f222eaa77f58751";

    public String execute(final SongInterface songInterface) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var url = "http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=" + API_KEY
                + "&artist=" + URLEncoder.encode(songInterface.getArtist().getName(), StandardCharsets.UTF_8)
                + "&album=" + URLEncoder.encode(songInterface.getAlbum().getName(), StandardCharsets.UTF_8)
                + "&format=json";

            final var request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .build();

            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            return handleResponse(response.body());
        } catch (final Exception exception) {
            logger().severe(exception.getMessage());
            exception.printStackTrace();
        }

        return "";
    }

    private String handleResponse(final String responseBody) {
        final var jsonResponse = new JSONObject(responseBody);

        if (!jsonResponse.has("album") || !jsonResponse.getJSONObject("album").has("image")) {
            return "";
        }

        return jsonResponse.getJSONObject("album")
            .getJSONArray("image")
            .getJSONObject(2)
            .getString("#text");
    }

}
