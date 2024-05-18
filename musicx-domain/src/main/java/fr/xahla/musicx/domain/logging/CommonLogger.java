package fr.xahla.musicx.domain.logging;

import lombok.Getter;
import org.jetbrains.annotations.PropertyKey;

import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.MissingResourceException;
import java.util.ResourceBundle;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Handles common logging messages within layers, using a resource bundle.
 * @author Cochetooo
 * @since 0.3.3
 */
public class CommonLogger {

    private final List<ResourceBundle> bundles;
    @Getter private final Logger logger;

    public CommonLogger(final Logger logger) {
        this.logger = logger;

        this.bundles = new ArrayList<>(
            List.of(ResourceBundle.getBundle("fr.xahla.musicx.domain.logging.log_message"))
        );
    }

    /**
     * @since 0.3.3
     */
    public void addResourceBundle(final ResourceBundle bundle) {
        if (bundles.contains(bundle)) {
            severe("DUPLICATE_RESOURCE", "ResourceBundle", bundle.getBaseBundleName());
            return;
        }

        bundles.add(bundle);
    }

    /**
     * @since 0.3.3
     */
    public void error(
        final Throwable exception,
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        logger.log(Level.SEVERE, MessageFormat.format(getString(key), args), exception);
    }

    /**
     * Logs a serious failure from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void severe(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.SEVERE, key, args);
    }

    /**
     * Logs a potential problem from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void warn(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.WARNING, key, args);
    }

    /**
     * Logs an informational message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void info(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.INFO, key, args);
    }

    /**
     * Logs a static configuration message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void config(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.CONFIG, key, args);
    }

    /**
     * Logs a tracing information message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void fine(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.FINE, key, args);
    }

    /**
     * Logs a fairly detailed message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void finer(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.FINER, key, args);
    }

    /**
     * Logs a highly detailed message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    public void finest(
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        log(Level.FINEST, key, args);
    }

    /**
     * Logs a message from the log message resource by specifying a key.<br>
     * If <code>key</code> value is not found, it will log the key name.
     * @throws NullPointerException If key is null.
     * @since 0.3.3
     */
    private void log(
        final Level level,
        @PropertyKey(resourceBundle = "fr.xahla.musicx.domain.logging.log_message") final String key,
        final Object ... args
    ) {
        try {
            logger.log(level, MessageFormat.format(getString(key), args));
        } catch (final MissingResourceException exception) {
            logger.log(level, "(Missing message text): " + key);
        }
    }

    /**
     * @throws MissingResourceException If key has not been found.
     * @since 0.3.3
     */
    private String getString(final String key) {
        for (final var bundle : this.bundles) {
            if (bundle.containsKey(key)) {
                return bundle.getString(key);
            }
        }

        throw new MissingResourceException("Key not found", "Logger", key);
    }

}
