package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.domain.service.fetch.FetchAlbumInterface;
import fr.xahla.musicx.domain.service.fetch.FetchArtistInterface;
import fr.xahla.musicx.domain.service.fetch.FetchSongInterface;
import org.json.JSONObject;

import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class LastFmRepository
    implements FetchArtistInterface, FetchAlbumInterface, FetchSongInterface
{

    private static final String API_KEY;
    private static final String API_URL;

    static {
        API_KEY = env("LASTFM_API_KEY");
        API_URL = env("LASTFM_API_URL");
    }

    /**
     * Fetch Info from an Artist:
     * <ul>
     *     <li>Artwork URL</li>
     * </ul>
     *
     * @see <a href="https://ws.audioscrobbler.com/2.0/?method=artist.getinfo">Get Info from Artist</a>
     * @param artist The artist source that will be modified then.
     * @param overwrite If true, overwrite data if already exists
     */
    @Override public void fetchArtistData(final ArtistInterface artist, final boolean overwrite) {
        final var methodSignature = "artist.getinfo";

        final var jsonResponse = this.sendRequest(
            methodSignature,
            Map.of(
                "api_key", API_KEY,
                "artist", URLEncoder.encode(artist.getName(), StandardCharsets.UTF_8)
            )
        );

        if (!jsonResponse.has("artist")) {
            logger().warning("No Last.FM artist has been found for artist: " + artist.getName());
            return;
        }

        final var artistJson = jsonResponse.getJSONObject("artist");

        // Artwork URL
        if (!overwrite || null == artist.getArtworkUrl() || artist.getArtworkUrl().isEmpty()) {
            artist.setArtworkUrl(artistJson
                .getJSONArray("image")
                .getJSONObject(3)
                .getString("#text")
            );
        }
    }

    /**
     * Fetch Info from an Album:
     * <ul>
     *     <li>Artwork URL</li>
     * </ul>
     *
     * @see <a href="https://ws.audioscrobbler.com/2.0/?method=album.getinfo">Get Info from Album</a>
     * @param album The album source that will be modified then.
     * @param overwrite If true, overwrite data if already exists
     */
    @Override public void fetchAlbumData(final AlbumInterface album, final boolean overwrite) {
        final var methodSignature = "album.getinfo";

        final var jsonResponse = this.sendRequest(
            methodSignature,
            Map.of(
                "api_key", API_KEY,
                "artist", URLEncoder.encode(album.getArtist().getName(), StandardCharsets.UTF_8),
                "album", URLEncoder.encode(album.getName(), StandardCharsets.UTF_8)
            )
        );

        if (!jsonResponse.has("album")) {
            logger().warning("No Last.FM album has been found for album: " +
                album.getArtist().getName() + " - " + album.getName());
            return;
        }

        final var albumJson = jsonResponse.getJSONObject("album");

        // Artwork URL
        if (!overwrite || null == album.getArtworkUrl() || album.getArtworkUrl().isEmpty()) {
            album.setArtworkUrl(albumJson
                .getJSONArray("image")
                .getJSONObject(3)
                .getString("#text")
            );
        }
    }

    /**
     * Fetch Info from a song:
     * <ul>
     *     <li><i>empty</i></li>
     * </ul>
     *
     * @see <a href="https://ws.audioscrobbler.com/2.0/?method=track.getinfo">Get Info from Song</a>
     * @param song The song source that will be modified then.
     * @param overwrite If true, overwrite data if already exists
     */
    @Override public void fetchSongData(final SongInterface song, final boolean overwrite) {
        final var methodSignature = "track.getinfo";

        final var jsonResponse = this.sendRequest(
            methodSignature,
            Map.of(
                "api_key", API_KEY,
                "artist", URLEncoder.encode(song.getArtist().getName(), StandardCharsets.UTF_8),
                "track", URLEncoder.encode(song.getTitle(), StandardCharsets.UTF_8)
            )
        );

        if (!jsonResponse.has("track")) {
            logger().warning("No Last.FM song has been found for song: " +
                song.getArtist().getName() + " - " + song.getTitle());
            return;
        }

        final var songJson = jsonResponse.getJSONObject("track");

        // @TODO
    }

    private JSONObject sendRequest(
        final String method,
        final Map<String, String> parameters
    ) {
        final var requestUrl = API_URL + "method=" + method + "&format=json&" + parameters.entrySet().stream()
            .map(entry -> "&" + entry.getKey() + "=" + URLEncoder.encode(entry.getValue(), StandardCharsets.UTF_8))
            .collect(Collectors.joining("&"));

        final var request = HttpRequest.newBuilder()
            .uri(URI.create(requestUrl))
            .build();

        try (final var httpClient = HttpClient.newHttpClient()) {
            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            return new JSONObject(response.body());
        } catch (final Exception exception) {
            throw new RuntimeException(exception);
        }
    }
}
