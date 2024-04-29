package fr.xahla.musicx.desktop;

import fr.xahla.musicx.desktop.manager.*;
import fr.xahla.musicx.desktop.model.Settings;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.Parent;

import java.io.IOException;
import java.util.Objects;
import java.util.logging.Level;
import java.util.logging.LogManager;
import java.util.logging.Logger;

/** <b>Global context for the desktop application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class DesktopContext extends AbstractContext {

    private ArtistListManager artistManager;
    private LoggerManager loggerManager;
    private QueueManager trackListManager;
    private TaskProgressManager taskProgressManager;

    private ObjectProperty<Parent> rightNavContent;

    private static DesktopContext context;

    protected DesktopContext(final Logger logger, final SettingsInterface settings, final AudioPlayerManagerInterface audioPlayer, final LibraryManagerInterface library) {
        super(logger, settings, audioPlayer, library);
    }

    public static void createContext() {
        context = new DesktopContext(
            Logger.getLogger(DesktopApplication.class.getName()),
            new Settings(),
            new AudioPlayerManager(),
            new LibraryManager()
        );

        try (var inputStream = Objects.requireNonNull(DesktopApplication.class.getResource("config/logger.properties")).openStream()) {
            LogManager.getLogManager().readConfiguration(inputStream);
        } catch (SecurityException | IOException | NullPointerException exception) {
            logger().severe(exception.getLocalizedMessage());
        }

        logger().setLevel(Level.ALL);

        context.loggerManager = new LoggerManager();
        context.taskProgressManager = new TaskProgressManager();
        context.trackListManager = new QueueManager();
        context.rightNavContent = new SimpleObjectProperty<>();

        // Requires library manager
        context.artistManager = new ArtistListManager();
        context.trackListManager.setQueue(library().getSongs(), 0);
    }

    public static ArtistListManager artist() {
        return context.artistManager;
    }

    public static LibraryManager library() {
        return (LibraryManager) context.libraryManager;
    }

    public static LoggerManager loggerManager() {
        return context.loggerManager;
    }

    public static AudioPlayerManager player() {
        return (AudioPlayerManager) context.audioPlayerManager;
    }

    public static Settings settings() {
        return (Settings) context.settings;
    }

    public static TaskProgressManager taskProgress() {
        return context.taskProgressManager;
    }

    public static QueueManager trackList() {
        return context.trackListManager;
    }

    public static ObjectProperty<Parent> rightNavContent() {
        return context.rightNavContent;
    }
}
