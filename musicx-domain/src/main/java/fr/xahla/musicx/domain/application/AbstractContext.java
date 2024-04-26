package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.repository.LastFmRepository;
import io.github.cdimascio.dotenv.Dotenv;

import java.util.logging.Logger;

public abstract class AbstractContext {

    protected final AbstractLogger logger;
    protected final SettingsInterface settings;
    protected final Dotenv env;

    protected final LastFmRepository lastFmRepository;

    protected final AudioPlayerManagerInterface audioPlayer;
    protected final LibraryManagerInterface library;

    protected static AbstractContext context;

    protected AbstractContext(
        final AbstractContext p_context,
        final AbstractLogger logger,
        final SettingsInterface settings,
        final AudioPlayerManagerInterface audioPlayer,
        final LibraryManagerInterface library
    ) {
        context = p_context;

        this.env = Dotenv.load();

        this.logger = logger;
        this.settings = settings;

        this.lastFmRepository = new LastFmRepository();

        this.audioPlayer = audioPlayer;
        this.library = library;
    }

    public static String env(final String key) {
        return context.env.get(key);
    }

    public static LastFmRepository lastFm() {
        return context.lastFmRepository;
    }

    public static Logger logger() {
        return context.logger.logger;
    }

}
