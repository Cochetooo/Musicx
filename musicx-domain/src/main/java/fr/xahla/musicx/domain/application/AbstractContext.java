package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.database.HibernateLoader;
import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import fr.xahla.musicx.domain.repository.*;
import io.github.cdimascio.dotenv.Dotenv;
import org.hibernate.Session;

import java.util.logging.Logger;

/**
 * Simple context that needs to be overridden by a superior layer.<br>
 * Gives a logger and an environment variable handler.
 * @author Cochetooo
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

    public static String env(final String key) {
        return context.env.get(key);
    }

    public static void log(final LogMessage message, final Object... args) {
        logger().log(message.level(), String.format(message.msg(), args));
    }

    public static Logger logger() {
        return context.logger;
    }

    public static Session openSession() {
        return context.hibernateLoader.getSession().openSession();
    }

    public static AlbumRepository albumRepository() {
        return context.albumRepository;
    }

    public static ArtistRepository artistRepository() {
        return context.artistRepository;
    }

    public static GenreRepository genreRepository() {
        return context.genreRepository;
    }

    public static LabelRepository labelRepository() {
        return context.labelRepository;
    }

    public static SongRepository songRepository() {
        return context.songRepository;
    }

}
