package fr.xahla.musicx.domain.service.apiHandler;

import org.json.JSONObject;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Common methods for external API handler classes
 * @author Cochetooo
 * @since 0.3.0
 */
public abstract class ExternalApiHandler {

    protected String apiUrl;

    protected ExternalApiHandler(final String apiUrl) {
        this.apiUrl = apiUrl;
    }

    /**
     * @since 0.3.0
     */
    protected abstract String makeURL(
        final String method,
        final Map<String, String> parameters
    );

    /**
     * @return A JSONObject with the content of the response, otherwise an empty JSONObject.
     * @since 0.3.0
     */
    protected JSONObject sendRequest(final String requestUrl) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var request = HttpRequest.newBuilder()
                .uri(URI.create(requestUrl))
                .build();

            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            return new JSONObject(response.body());
        } catch (final IOException | InterruptedException exception) {
            logger().error(exception, "API_CALL_ERROR", requestUrl);

            return new JSONObject();
        }
    }

}
