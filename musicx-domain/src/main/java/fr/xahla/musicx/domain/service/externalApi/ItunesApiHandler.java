package fr.xahla.musicx.domain.service.externalApi;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.domain.repository.ExternalFetchRepositoryInterface;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class ItunesApiHandler
    extends ExternalApiHandler
    implements ExternalFetchRepositoryInterface.AlbumFetcher
{

    public ItunesApiHandler() {
        super(env("ITUNES_API_URL"));
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
    @Override public void fetchAlbumFromExternal(final AlbumInterface album, final boolean overwrite) {
        final var methodSignature = "search";

        final var searchTerm = album.getArtist().getName() + " " + album.getName();

        final var requestUrl = this.makeURL(
            methodSignature,
            Map.of(
                "term", searchTerm,
                "media", "music",
                "entity", "album",
                "limit", "1"
            )
        );

        final var jsonResponse = this.sendRequest(requestUrl);

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
                DateTimeFormatter.ISO_DATE_TIME
            ).toLocalDate());
        }

        // Primary Genre placeholder
        if (!overwrite || null == album.getPrimaryGenres() || album.getPrimaryGenres().isEmpty()) {
            //album.setPrimaryGenres(new ArrayList<>()); @TODO
        }
    }

    @Override protected String makeURL(final String method, final Map<String, String> parameters) {
        return this.apiUrl + method + "?" + parameters.entrySet().stream()
            .map(entry -> "&" + entry.getKey() + "=" + URLEncoder.encode(entry.getValue(), StandardCharsets.UTF_8))
            .collect(Collectors.joining("&"));
    }
}
