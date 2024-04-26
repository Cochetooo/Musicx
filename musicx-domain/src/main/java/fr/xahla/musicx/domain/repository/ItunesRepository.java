package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.domain.service.fetch.FetchAlbumInterface;
import org.json.JSONObject;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class ItunesRepository
    implements FetchAlbumInterface
{

    private static final String API_URL;

    static {
        API_URL = env("ITUNES_API_URL");
    }

    /**
     * Fetch Info from an Album:
     * <ul>
     *     <li>Artwork URL</li>
     *     <li>Release Date</li>
     *     <li>Primary Placeholder Genre</li>
     * </ul>
     *
     * @see <a href="https://itunes.apple.com/search">Get Info from Album</a>
     * @param album The album source that will be modified then.
     * @param overwrite If true, overwrite data if already exists
     */
    @Override public void fetchAlbumData(final AlbumInterface album, final boolean overwrite) {
        final var methodSignature = "search";

        final var searchTerm = album.getArtist().getName() + " " + album.getName();

        final var jsonResponse = this.sendRequest(
            methodSignature,
            Map.of(
                "term", searchTerm,
                "media", "music",
                "entity", "album",
                "limit", "1"
            )
        );

        if (!jsonResponse.has("results") || jsonResponse.getJSONArray("results").isEmpty()) {
            logger().warning("No results found for searchTerm: " + searchTerm);
            return;
        }

        final var albumJson = jsonResponse.getJSONArray("results").getJSONObject(0);

        // Artwork URL
        if (!overwrite || null == album.getArtworkUrl() || album.getArtworkUrl().isEmpty()) {
            album.setArtworkUrl(albumJson.getString("artworkUrl100"));
        }

        // Release Date
        if (!overwrite || null == album.getReleaseDate()) {
            album.setReleaseDate(ZonedDateTime.parse(
                albumJson.getString("releaseDate"),
                DateTimeFormatter.ISO_INSTANT
            ).toLocalDate());
        }

        // Primary Genre placeholder
        if (!overwrite || null == album.getPrimaryGenres() || album.getPrimaryGenres().isEmpty()) {
            album.setPrimaryGenres(List.of(

            ));
        }
    }

    private JSONObject sendRequest(
        final String method,
        final Map<String, String> parameters
    ) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var requestUrl = API_URL + method + "?" + parameters.entrySet().stream()
                .map(entry -> "&" + entry.getKey() + "=" + URLEncoder.encode(entry.getValue(), StandardCharsets.UTF_8))
                .collect(Collectors.joining("&"));

            final var request = HttpRequest.newBuilder()
                .uri(URI.create(requestUrl))
                .build();

            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            return new JSONObject(response.body());
        } catch (final Exception exception) {
            throw new RuntimeException(exception);
        }
    }
}
