package fr.xahla.musicx.desktop.logging;

import fr.xahla.musicx.domain.logging.LogMessage;

import java.util.logging.Level;

/**
 * Lists of all used logging message in the desktop application layer.
 * @author Cochetooo
 * @since 0.2.0
 */
public class LogMessageFX {

    /*
     * Errors
     */
    public static LogMessage ERROR_FXML_COMPONENT_LOAD = new LogMessage("Unable to load FXML component {0}.", Level.SEVERE);
    public static LogMessage ERROR_FXML_MODAL_LOAD = new LogMessage("Unable to load FXML modal {0}.", Level.SEVERE);

    /*
     * Warnings
     */
    public static LogMessage WARNING_MEDIA_PLAYER_NOT_CREATED = new LogMessage("Trying to access media player while not created.", Level.WARNING);

    /*
     * Fine
     */
    public static LogMessage FINE_QUEUE_POSITION_OUT_OF_BOUNDS = new LogMessage("Trying to change queue position but out of bounds: {0} out of {1} elements.", Level.FINE);
    public static LogMessage FINE_QUEUE_SONG_OUT_OF_BOUNDS = new LogMessage("Trying to access out of bounds song position {0} in a queue of {1} elements.", Level.FINE);

    /*
    FOLDER_PATH_NOT_DIRECTORY("{0} path is not a directory."),
    LIBRARY_NOT_INITIALIZED("Library has not been initialized when {0} try to access it."),
    MEDIA_PLAYER_NOT_CREATED("Trying to access media player while not created."),
    MEDIA_PLAYER_NOT_CREATED_STOP("Stopping while media player is inactive."),
    SONG_NOT_FOUND_IN_LIBRARY("The song has not been found within the library: {0}"),
    THREAD_INTERRUPTED_INFO("The thread for {0} has been interrupted by a new call on this method.");
    */

}
