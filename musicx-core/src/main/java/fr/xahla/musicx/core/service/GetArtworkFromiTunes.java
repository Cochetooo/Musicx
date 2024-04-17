package fr.xahla.musicx.core.service;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.util.Optional;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class GetArtworkFromiTunes {

    public String execute(final String searchTerm) {
        try (final var httpClient = HttpClient.newHttpClient()) {
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

    private Optional<String> extractArtworkUrl(final String jsonResponse) {
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
