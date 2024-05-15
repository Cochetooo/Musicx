package fr.xahla.musicx.domain.service.apiHandler;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriteria;
import fr.xahla.musicx.domain.repository.data.ExternalFetchRepositoryInterface;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;

public class ItunesApiHandler
    extends ExternalApiHandler
    implements ExternalFetchRepositoryInterface.AlbumFetcher
{
    private static final ItunesApiHandler INSTANCE = new ItunesApiHandler();

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
    @Override public void fetchAlbumFromExternal(final AlbumDto album, final boolean overwrite) {
        final var methodSignature = "search";

        final var artist = albumRepository().getArtist(album);

        var searchTerm = album.getName();

        if (null != artist) {
            searchTerm += " " + artist.getName();
        }

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
        if (overwrite || null == album.getArtworkUrl() || album.getArtworkUrl().isEmpty()) {
            album.setArtworkUrl(albumJson.getString("artworkUrl100"));
        }

        // Primary Genre placeholder
        if (overwrite || null == album.getPrimaryGenreIds() || album.getPrimaryGenreIds().isEmpty()) {
            final var itunesGenreName = albumJson.getString("primaryGenreName");

            final var genre = genreRepository().findByCriteria(Map.of(
                GenreSearchCriteria.NAME, itunesGenreName
            ));

            if (!genre.isEmpty()) {
                album.setPrimaryGenreIds(List.of(
                    genre.getFirst().getId()
                ));
            } else {
                logger().info("Invalid genre name from ITunes: " + itunesGenreName + " for album: " + album.getName());
            }
        }

        // Release Date
        if (overwrite || null == album.getReleaseDate()) {
            album.setReleaseDate(ZonedDateTime.parse(
                albumJson.getString("releaseDate"),
                DateTimeFormatter.ISO_DATE_TIME
            ).toLocalDate());
        }

        // Track Total
        if (overwrite || 0 == album.getTrackTotal()) {
            album.setTrackTotal((short) albumJson.getInt("trackCount"));
        }
    }

    @Override protected String makeURL(final String method, final Map<String, String> parameters) {
        return this.apiUrl + method + "?" + parameters.entrySet().stream()
            .map(entry -> "&" + entry.getKey() + "=" + URLEncoder.encode(entry.getValue(), StandardCharsets.UTF_8))
            .collect(Collectors.joining("&"));
    }

    public static ItunesApiHandler itunesApi() {
        return INSTANCE;
    }
}
