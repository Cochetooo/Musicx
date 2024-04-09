package fr.xahla.musicx.desktop;

import fr.xahla.musicx.desktop.manager.ArtistManager;
import fr.xahla.musicx.desktop.manager.LibraryManager;
import fr.xahla.musicx.desktop.manager.PlayerManager;
import fr.xahla.musicx.desktop.manager.QueueManager;

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
