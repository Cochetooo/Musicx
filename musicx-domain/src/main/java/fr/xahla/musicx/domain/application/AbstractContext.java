package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.service.externalApi.ItunesApiHandler;
import fr.xahla.musicx.domain.service.externalApi.LastFmApiHandler;
import io.github.cdimascio.dotenv.Dotenv;

import java.util.logging.Logger;

public class AbstractContext {

    protected final Logger logger;
    protected final SettingsInterface settings;
    protected final Dotenv env;

    protected final LastFmApiHandler lastFmApiHandler;
    protected final ItunesApiHandler itunesApiHandler;

    protected final AudioPlayerManagerInterface audioPlayerManager;
    protected final LibraryManagerInterface libraryManager;

    protected static AbstractContext context;

    protected AbstractContext(
        final Logger logger,
        final SettingsInterface settings,
        final AudioPlayerManagerInterface audioPlayerManager,
        final LibraryManagerInterface libraryManager
    ) {
        context = this;

        this.env = Dotenv.load();

        this.logger = logger;
        this.settings = settings;

        this.lastFmApiHandler = new LastFmApiHandler();
        this.itunesApiHandler = new ItunesApiHandler();

        this.audioPlayerManager = audioPlayerManager;
        this.libraryManager = libraryManager;
    }

    public static String env(final String key) {
        return context.env.get(key);
    }

    public static ItunesApiHandler iTunesApi() {
        return context.itunesApiHandler;
    }

    public static LastFmApiHandler lastFmApi() {
        return context.lastFmApiHandler;
    }

    public static Logger logger() {
        return context.logger;
    }

}
