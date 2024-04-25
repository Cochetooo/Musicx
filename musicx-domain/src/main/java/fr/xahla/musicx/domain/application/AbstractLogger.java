package fr.xahla.musicx.domain.application;

import java.util.logging.Logger;

public abstract class AbstractLogger {

    protected final Logger logger;

    protected AbstractLogger(final Logger logger) {
        this.logger = logger;
    }

}
