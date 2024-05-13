package fr.xahla.musicx.desktop;

import fr.xahla.musicx.desktop.manager.*;
import fr.xahla.musicx.desktop.model.Settings;
import fr.xahla.musicx.domain.application.AbstractContext;
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
    private QueueManager trackListManager;
    private TaskProgressManager taskProgressManager;

    private LibraryManager libraryManager;
    private Settings settings;
    private AudioPlayerManager audioPlayerManager;

    private RightNavContentManager rightNavContent;

    private static DesktopContext context;

    protected DesktopContext(final Logger logger) {
        super(logger);

        this.libraryManager = new LibraryManager();
        this.settings = new Settings();
        this.audioPlayerManager = new AudioPlayerManager();
    }

    public static void createContext() {
        final var logger = Logger.getLogger(DesktopContext.class.getName());
        logger.setLevel(Level.ALL);

        context = new DesktopContext(
            logger
        );

        context.taskProgressManager = new TaskProgressManager();
        context.trackListManager = new QueueManager();
        context.rightNavContent = new RightNavContentManager();

        // Requires library manager
        context.artistManager = new ArtistListManager();
        context.trackListManager.setQueue(library().getSongs(), 0);
    }

    public static ArtistListManager artist() {
        return context.artistManager;
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
}
