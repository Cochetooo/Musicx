package fr.xahla.musicx.infrastructure.service.artistThumb;

import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.infrastructure.model.data.enums.SoftwareInfo;
import org.json.JSONObject;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;

import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Service to retrieve an artist thumb from LastFM</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class GetArtistThumbFromLastFm {

    public static String execute(final ArtistInterface artistInterface) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var url = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&api_key=" + SoftwareInfo.LASTFM_TOKEN.getInfo()
                + "&artist=" + URLEncoder.encode(artistInterface.getName(), StandardCharsets.UTF_8)
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

    private static String handleResponse(final String responseBody) {
        final var jsonResponse = new JSONObject(responseBody);

        if (!jsonResponse.has("artist") || !jsonResponse.getJSONObject("artist").has("image")) {
            return "";
        }

        return jsonResponse.getJSONObject("artist")
            .getJSONArray("image")
            .getJSONObject(2)
            .getString("#text");
    }

}
