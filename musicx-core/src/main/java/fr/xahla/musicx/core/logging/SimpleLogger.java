package fr.xahla.musicx.core.logging;

import java.util.logging.Logger;

public class SimpleLogger {

    private static Logger logger;

    public static void setLogger(final Logger _logger) {
        logger = _logger;
    }

    public static Logger logger() {
        return logger;
    }

}
