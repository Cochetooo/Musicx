package fr.xahla.musicx.infrastructure.service.albumArtwork;

import fr.xahla.musicx.api.model.SongInterface;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.util.Optional;

import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Service to retrieve an album artwork from iTunes</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class GetArtworkFromiTunes {

    public static String execute(final SongInterface song) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var searchTerm = song.getArtist().getName() + " " + song.getAlbum().getName();

            final var url = "https://itunes.apple.com/search?term=" + URLEncoder.encode(searchTerm, StandardCharsets.UTF_8) + "&media=music";

            final var request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .build();

            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            final var artworkUrl = extractArtworkUrl(response.body());

            final String[] result = {""};

            artworkUrl.ifPresent(responseUrl -> result[0] = responseUrl);

            return result[0];
        } catch (final Exception exception) {
            logger().severe(exception.getMessage());
            exception.printStackTrace();
        }

        return "";
    }

    private static Optional<String> extractArtworkUrl(final String jsonResponse) {
        var startIndex = jsonResponse.indexOf("artworkUrl100");

        if (-1 == startIndex) {
            return Optional.of("");
        }

        startIndex +=  "artworkUrl100".length() + 3;

        final var endIndex = jsonResponse.indexOf("\"", startIndex);
        final var url = jsonResponse.substring(startIndex, endIndex);
        return Optional.of(url);
    }

}
