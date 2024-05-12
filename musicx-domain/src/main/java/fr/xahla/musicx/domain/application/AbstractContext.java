package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.database.HibernateLoader;
import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import io.github.cdimascio.dotenv.Dotenv;

import java.util.logging.Logger;

public class AbstractContext {

    protected final Logger logger;
    protected final Dotenv env;

    protected static AbstractContext context;

    protected AbstractContext(
        final Logger logger
    ) {
        context = this;

        this.env = Dotenv.load();

        this.logger = logger;
        logger.addHandler(new SplitConsoleHandler());
    }

    public static String env(final String key) {
        return context.env.get(key);
    }

    public static Logger logger() {
        return context.logger;
    }

}
