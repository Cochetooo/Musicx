package fr.xahla.musicx.domain.logging;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Log unhandled exception according to the application logger logic.
 * @author Cochetooo
 * @since 0.3.3
 */
public class ExceptionHandler implements Thread.UncaughtExceptionHandler {

    @Override public void uncaughtException(final Thread thread, final Throwable exception) {
        logger().error(exception, "UNHANDLED_EXCEPTION", exception.getLocalizedMessage());
    }

}
