package fr.xahla.musicx.desktop;

import fr.xahla.musicx.desktop.manager.*;
import fr.xahla.musicx.desktop.model.Settings;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.Parent;

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
public class DesktopContext {

    private static DesktopContext context;

    private ArtistManager artistManager;
    private LibraryManager libraryManager;
    private LoggerManager loggerManager;
    private PlayerManager playerManager;
    private QueueManager trackListManager;
    private TaskProgressManager taskProgressManager;

    private Settings settings;
    private ObjectProperty<Parent> rightNavContent;

    public static void createContext(
        final LoggerManager loggerManager
    ) {
        context = new DesktopContext();

        context.loggerManager = loggerManager;
        context.settings = new Settings();
        context.taskProgressManager = new TaskProgressManager();

        context.libraryManager = new LibraryManager();
        context.playerManager = new PlayerManager();
        context.trackListManager = new QueueManager();

        context.rightNavContent = new SimpleObjectProperty<>();

        // Requires library manager
        context.artistManager = new ArtistManager();
        context.trackListManager.setQueue(library().getSongs(), 0);
    }

    public static ArtistManager artist() {
        return context.artistManager;
    }

    public static LibraryManager library() {
        return context.libraryManager;
    }

    public static LoggerManager loggerManager() {
        return context.loggerManager;
    }

    public static PlayerManager player() {
        return context.playerManager;
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

    public static ObjectProperty<Parent> rightNavContent() {
        return context.rightNavContent;
    }
}
