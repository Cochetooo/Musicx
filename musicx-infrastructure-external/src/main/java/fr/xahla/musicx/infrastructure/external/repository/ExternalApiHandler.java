package fr.xahla.musicx.infrastructure.external.repository;

import org.json.JSONObject;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Map;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public abstract class ExternalApiHandler {

    protected String apiUrl;

    protected ExternalApiHandler(final String apiUrl) {
        this.apiUrl = apiUrl;
    }

    protected abstract String makeURL(
        final String method,
        final Map<String, String> parameters
    );

    protected JSONObject sendRequest(final String requestUrl) {
        try (final var httpClient = HttpClient.newHttpClient()) {
            final var request = HttpRequest.newBuilder()
                .uri(URI.create(requestUrl))
                .build();

            final var response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
            return new JSONObject(response.body());
        } catch (final IOException | InterruptedException exception) {
            logger().log(Level.SEVERE, "Couldn't call API: " + apiUrl, exception);
            return new JSONObject();
        }
    }

}
