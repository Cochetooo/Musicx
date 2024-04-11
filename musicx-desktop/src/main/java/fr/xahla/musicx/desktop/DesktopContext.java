package fr.xahla.musicx.desktop;

import fr.xahla.musicx.desktop.manager.ArtistManager;
import fr.xahla.musicx.desktop.manager.LibraryManager;
import fr.xahla.musicx.desktop.manager.PlayerManager;
import fr.xahla.musicx.desktop.manager.QueueManager;

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
    private PlayerManager playerManager;
    private QueueManager trackListManager;

    public static void createContext() {
        context = new DesktopContext();
        context.libraryManager = new LibraryManager();
        context.playerManager = new PlayerManager();
        context.trackListManager = new QueueManager();

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

    public static PlayerManager player() {
        return context.playerManager;
    }

    public static QueueManager trackList() {
        return context.trackListManager;
    }

}
