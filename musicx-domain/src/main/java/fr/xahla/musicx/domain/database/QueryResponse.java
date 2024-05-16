package fr.xahla.musicx.domain.database;

import fr.xahla.musicx.domain.logging.LogMessage;

import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.log;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Container for a query response used to properly manage the response.
 * @param response
 * @author Cochetooo
 */
public record QueryResponse(
    List<?> response
) {

    /**
     * @return The first object of the list
     * @throws NullPointerException if the response is empty
     */
    public Object unique() {
        if (response.isEmpty()) {
            log(LogMessage.ERROR_QUERY_NULL_RESPONSE);
            throw new NullPointerException();
        }

        return response.getFirst();
    }

    /**
     * @return The first object of the list or <b>null</b> if the list is empty
     */
    public Object uniqueOrNull() {
        if (response.isEmpty()) {
            return null;
        }

        return response.getFirst();
    }

}
