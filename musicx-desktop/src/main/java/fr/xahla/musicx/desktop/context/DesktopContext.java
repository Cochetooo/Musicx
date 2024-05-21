package fr.xahla.musicx.desktop.context;

import fr.xahla.musicx.desktop.manager.*;
import fr.xahla.musicx.desktop.context.scene.settings.Settings;
import fr.xahla.musicx.domain.application.AbstractContext;

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

    private ArtistListManager artistManager;
    private QueueManager trackListManager;
    private TaskProgressManager taskProgressManager;

    private LibraryManager libraryManager;
    private Settings settings;
    private AudioPlayerManager audioPlayerManager;

    private RightNavContentManager rightNavContent;

    private Controller contextController;

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

        context.config = new Config();
        context.contextController = new Controller();

        context.libraryManager = new LibraryManager();
        context.settings = new Settings();
        context.audioPlayerManager = new AudioPlayerManager();

        context.commonLogger.addResourceBundle(ResourceBundle.getBundle("fr.xahla.musicx.desktop.config.log_message_fx"));

        context.taskProgressManager = new TaskProgressManager();
        context.trackListManager = new QueueManager();
        context.rightNavContent = new RightNavContentManager();

        // Requires library manager
        context.artistManager = new ArtistListManager();

        context.trackListManager.setQueue(library().getSongs(), 0);

        library().onSongsChange(change -> {
            context.trackListManager.setQueue(library().getSongs(), 0);
        });
    }

    public static ArtistListManager artist() {
        return context.artistManager;
    }

    public static Config config() {
        return context.config;
    }

    public static LibraryManager library() {
        return context.libraryManager;
    }

    public static AudioPlayerManager player() {
        return context.audioPlayerManager;
    }

    public static Settings settings() {
        return context.settings;
    }

    public static TaskProgressManager taskProgress() {
        return context.taskProgressManager;
    }

    public static QueueManager trackList() {
        return context.trackListManager;
    }

    public static RightNavContentManager rightNavContent() {
        return context.rightNavContent;
    }

    public static Controller controller() {
        return context.contextController;
    }
}
