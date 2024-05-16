package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.database.HibernateLoader;
import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import fr.xahla.musicx.domain.repository.*;
import io.github.cdimascio.dotenv.Dotenv;
import org.hibernate.Session;

import java.text.MessageFormat;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Simple context that needs to be overridden by a superior layer.<br>
 * Gives a logger and an environment variable handler.
 * @author Cochetooo
 * @since 0.3.0
 */
public class AbstractContext {

    protected final Logger logger;
    protected final Dotenv env;
    protected final HibernateLoader hibernateLoader;

    protected final AlbumRepository albumRepository;
    protected final ArtistRepository artistRepository;
    protected final GenreRepository genreRepository;
    protected final LabelRepository labelRepository;
    protected final SongRepository songRepository;

    protected static AbstractContext context;

    protected AbstractContext(
        final Logger logger
    ) {
        context = this;

        this.env = Dotenv.load();

        this.logger = logger;
        logger.addHandler(new SplitConsoleHandler());

        this.hibernateLoader = new HibernateLoader();

        this.albumRepository = new AlbumRepository();
        this.artistRepository = new ArtistRepository();
        this.genreRepository = new GenreRepository();
        this.labelRepository = new LabelRepository();
        this.songRepository = new SongRepository();
    }

    /**
     * @return The value stored in the environment file if the key is found, otherwise the key.
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.0
     */
    public static String env(final String key) {
        checkInitialization("env");

        return context.env.get(key);
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static void error(final Exception exception, final LogMessage message, final Object... args) {
        checkInitialization("error");

        final var logMessage = MessageFormat.format(message.msg(), args);

        logger().log(
            Level.SEVERE,
            logMessage,
            exception
        );
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static void log(final LogMessage message, final Object... args) {
        checkInitialization("log");

        final var logMessage = MessageFormat.format(message.msg(), args);
        logger().log(message.level(), logMessage);
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.0
     */
    public static Logger logger() {
        checkInitialization("logger");

        return context.logger;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static Session openSession() {
        checkInitialization("openSession");

        return context.hibernateLoader.getSession().openSession();
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static AlbumRepository albumRepository() {
        checkInitialization("albumRepository");

        return context.albumRepository;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static ArtistRepository artistRepository() {
        checkInitialization("artistRepository");

        return context.artistRepository;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static GenreRepository genreRepository() {
        checkInitialization("genreRepository");

        return context.genreRepository;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static LabelRepository labelRepository() {
        checkInitialization("labelRepository");

        return context.labelRepository;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    public static SongRepository songRepository() {
        checkInitialization("songRepository");

        return context.songRepository;
    }

    /**
     * @throws ExceptionInInitializerError If context has not been initialized
     * @since 0.3.2
     */
    private static void checkInitialization(final String method) {
        if (null == context) {
            throw new ExceptionInInitializerError("Must initialize context before exploiting global method: " + method);
        }
    }

}
