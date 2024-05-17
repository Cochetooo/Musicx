package fr.xahla.musicx.domain.logging;

import java.util.ResourceBundle;

/**
 * Handles common logging messages within layers, using a resource bundle.
 * @author Cochetooo
 * @since 0.3.3
 */
public class CommonLogger {

    private final ResourceBundle standardBundle;

    public CommonLogger() {
        this.standardBundle = ResourceBundle.getBundle("fr.xahla.musicx.domain.logging.log_message");
    }

    public void addResourceBundle(final ResourceBundle bundle) {
        // @TODO
    }

    public void log(final String key, final Object ... args) {

    }

}
