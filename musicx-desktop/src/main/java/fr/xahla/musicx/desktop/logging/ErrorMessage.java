package fr.xahla.musicx.desktop.logging;

import java.text.MessageFormat;

public enum ErrorMessage {

    BENCHMARK("Benchmark execution time for {0} : {1}ms."),

    FOLDER_PATH_NOT_DIRECTORY("{0} path is not a directory."),
    LIBRARY_NOT_INITIALIZED("Library has not been initialized when {0} try to access it."),
    LOAD_FXML_COMPONENT_ERROR("Unable to load FXML component {0}."),
    LOAD_FXML_MODAL_ERROR("Unable to load modal {0}."),
    MEDIA_PLAYER_NOT_CREATED("Trying to access media player while not created."),
    MEDIA_PLAYER_NOT_CREATED_STOP("Stopping while media player is inactive."),
    QUEUE_POSITION_OUT_OF_BOUNDS("Trying to change queue position but out of bounds: {0} out of {1} elements."),
    QUEUE_SONG_OUT_OF_BOUNDS("Trying to access out of bounds song position {0} in a queue of {1} elements."),
    SONG_NOT_FOUND_IN_LIBRARY("The song has not been found within the library: {0}"),
    THREAD_INTERRUPTED_INFO("The thread for {0} has been interrupted by a new call on this method.");

    String msg;

    ErrorMessage(final String msg) {
        this.msg = msg;
    }

    public String getMsg() {
        return this.msg;
    }

    public String getMsg(final Object... args) {
        return MessageFormat.format(this.getMsg(), args);
    }

}
