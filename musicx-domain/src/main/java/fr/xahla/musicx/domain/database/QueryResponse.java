package fr.xahla.musicx.domain.database;

import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public record QueryResponse(
    List<?> response
) {

    Object unique() {
        if (response.isEmpty()) {
            logger().severe("Trying to get unique object of null response");
            throw new NullPointerException();
        }

        return response.getFirst();
    }

    Object uniqueOrNull() {
        if (response.isEmpty()) {
            return null;
        }

        return response.getFirst();
    }

}
