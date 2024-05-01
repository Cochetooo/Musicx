package fr.xahla.musicx.infrastructure.external.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.domain.repository.ExternalFetchRepositoryInterface;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public class LastFmApiHandler
    extends ExternalApiHandler
    implements ExternalFetchRepositoryInterface.ArtistFetcher, ExternalFetchRepositoryInterface.SongFetcher, ExternalFetchRepositoryInterface.AlbumFetcher
{
    private final String apiKey;

    public LastFmApiHandler() {
        super(env("LASTFM_API_URL"));
        this.apiKey = env("LASTFM_API_KEY");
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
    @Override public void fetchArtistFromExternal(final ArtistDto artist, final boolean overwrite) {
        final var methodSignature = "artist.getinfo";

        final var requestUrl = this.makeURL(
            methodSignature,
            Map.of(
                "api_key", this.apiKey,
                "artist", artist.getName()
            )
        );

        final var jsonResponse = this.sendRequest(requestUrl);

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
    @Override public void fetchAlbumFromExternal(final AlbumDto album, final boolean overwrite) {
        final var methodSignature = "album.getinfo";

        final var requestUrl = this.makeURL(
            methodSignature,
            Map.of(
                "api_key", this.apiKey,
                "artist", album.getArtist().getName(),
                "album", album.getName()
            )
        );

        final var jsonResponse = this.sendRequest(requestUrl);

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
    @Override public void fetchSongFromExternal(final SongDto song, final boolean overwrite) {
        final var methodSignature = "track.getinfo";

        final var requestUrl = this.makeURL(
            methodSignature,
            Map.of(
                "api_key", this.apiKey,
                "artist", song.getArtist().getName(),
                "track", song.getTitle()
            )
        );

        final var jsonResponse = this.sendRequest(requestUrl);

        if (!jsonResponse.has("track")) {
            logger().warning("No Last.FM song has been found for song: " +
                song.getArtist().getName() + " - " + song.getTitle());
            return;
        }

        final var songJson = jsonResponse.getJSONObject("track");

        // @TODO
    }

    @Override protected String makeURL(final String method, final Map<String, String> parameters) {
        return this.apiUrl + "method=" + method + "&format=json&" + parameters.entrySet().stream()
            .map(entry -> "&" + entry.getKey() + "=" + URLEncoder.encode(entry.getValue(), StandardCharsets.UTF_8))
            .collect(Collectors.joining("&"));
    }
}
