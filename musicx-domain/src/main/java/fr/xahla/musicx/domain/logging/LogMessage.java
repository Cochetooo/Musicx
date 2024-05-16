package fr.xahla.musicx.domain.logging;

import java.util.logging.Level;

/**
 * Lists of all used logging message in the domain layer.
 * @author Cochetooo
 */
public record LogMessage(String msg, Level level) {

    /*
     * Errors
     */
    public static final LogMessage ERROR_API_REQUEST = new LogMessage("Could not call API: {0}", Level.SEVERE);
    public static final LogMessage ERROR_AUDIO_TAGGER_CUSTOM_TAG_FETCH = new LogMessage("Error while getting custom tag {0}", Level.SEVERE);
    public static final LogMessage ERROR_AUDIO_TAGGER_CUSTOM_TAG_WRITE = new LogMessage("Could not set custom tag {0} with value {1}", Level.SEVERE);
    public static final LogMessage ERROR_AUDIO_TAGGER_READ_FILE = new LogMessage("Could not read audio file: {0}", Level.SEVERE);
    public static final LogMessage ERROR_HIBERNATE_INITIALIZATION = new LogMessage("Could not initialize Hibernate and its session factory.", Level.SEVERE);
    public static final LogMessage ERROR_IO_NOT_VALID_URI = new LogMessage("{0} location is not a valid URI.", Level.SEVERE);
    public static final LogMessage ERROR_IO_FILE_NOT_FOUND = new LogMessage("{0} location is not a valid file path.", Level.SEVERE);
    public static final LogMessage ERROR_IO_SAVE = new LogMessage("Unable to save file: {0}", Level.SEVERE);
    public static final LogMessage ERROR_JSON_NOT_VALID = new LogMessage("{0} is not a valid JSON.", Level.SEVERE);
    public static final LogMessage ERROR_REPOSITORY_SAVE = new LogMessage("Error while persisting {0}", Level.SEVERE);
    public static final LogMessage ERROR_QUERY = new LogMessage("An exception has occured while trying to query: {0}", Level.SEVERE);
    public static final LogMessage ERROR_QUERY_NO_TABLE_DEFINED = new LogMessage("No table has been defined for query: {0}", Level.SEVERE);
    public static final LogMessage ERROR_QUERY_NULL_RESPONSE = new LogMessage("Trying to get object of null response", Level.SEVERE);

    /*
     * Warning
     */
    public static final LogMessage WARNING_API_RESPONSE_EMPTY = new LogMessage("No result for {0}: {1}", Level.WARNING);
    public static final LogMessage WARNING_AUDIO_TAGGER_CUSTOM_TAG_FORMAT_NOT_SUPPORTED = new LogMessage("Could not set custom tag {0} because tag format is not supported.", Level.WARNING);
    public static final LogMessage WARNING_REPOSITORY_ITEM_NOT_FOUND = new LogMessage("{0} not found with {1}: {2}", Level.WARNING);

    /*
     * Info
     */
    public static final LogMessage INFO_API_INVALID_DATA = new LogMessage("Invalid {0} from API {1}: {2} for {3}: {4}", Level.INFO);
    public static final LogMessage INFO_BENCHMARK = new LogMessage("{0} in {1} ms.", Level.INFO);
    public static final LogMessage INFO_IO_NO_FILES_FOUND = new LogMessage("No {0} found in {1}.", Level.INFO);
    public static final LogMessage INFO_HIBERNATE_SQL_STATEMENT = new LogMessage("[Hibernate SQL] {0}", Level.INFO);

    /*
     * Fine
     */
    public static final LogMessage FINE_REPOSITORY_SAVE_SUCCESS = new LogMessage("{0} saved successfully: {1}", Level.INFO);

    /*
     * Finer
     */
    public static final LogMessage FINER_AUDIO_TAGGER_NULL_CUSTOM_KEY = new LogMessage("Null value for custom key {0}", Level.FINER);

}
