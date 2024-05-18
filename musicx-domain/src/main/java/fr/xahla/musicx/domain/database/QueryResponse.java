package fr.xahla.musicx.domain.database;

import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Container for a query response used to properly manage the response.
 * @author Cochetooo
 * @since 0.3.1
 */
public record QueryResponse(
    List<?> response
) {

    /**
     * @return The first object of the list
     * @throws NullPointerException if the response is empty
     * @since 0.3.1
     */
    public Object unique() {
        if (response.isEmpty()) {
            logger().severe("QUERY_NULL_RESPONSE");
            throw new NullPointerException();
        }

        return response.getFirst();
    }

    /**
     * @return The first object of the list or <b>null</b> if the list is empty
     * @since 0.3.1
     */
    public Object uniqueOrNull() {
        if (response.isEmpty()) {
            return null;
        }

        return response.getFirst();
    }

}
