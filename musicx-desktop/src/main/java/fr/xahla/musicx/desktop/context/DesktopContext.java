package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.manager.AudioPlayerManager;
import fr.xahla.musicx.domain.application.AbstractContext;
import javafx.stage.Stage;

import java.util.ResourceBundle;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Global context for the desktop application.
 * @author Cochetooo
 * @since 0.1.0
 */
public class DesktopContext extends AbstractContext {

    private Config config;
    private AudioPlayerManager audioPlayerManager;
    private SceneController sceneController;

    private static DesktopContext context;

    protected DesktopContext(final Logger logger) {
        super(logger);
    }

    public static void createContext() {
        final var logger = Logger.getLogger(DesktopContext.class.getName());
        logger.setLevel(Level.ALL);

        context = new DesktopContext(
            logger
        );

        context.commonLogger.addResourceBundle(ResourceBundle.getBundle("fr.xahla.musicx.desktop.config.log_message_fx"));

        context.config = new Config();
        context.sceneController = new SceneController();

        context.audioPlayerManager = new AudioPlayerManager();
    }

    public static Config config() {
        return context.config;
    }

    public static AudioPlayerManager audioPlayer() {
        return context.audioPlayerManager;
    }

    public static SceneController scene() {
        return context.sceneController;
    }
}
