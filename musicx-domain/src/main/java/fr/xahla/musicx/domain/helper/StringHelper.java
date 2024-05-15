package fr.xahla.musicx.domain.helper;

/**
 * Utility class for String
 * @author Cochetooo
 */
public final class StringHelper {

    /**
     * @return The parsed long or 0 if the format is incorrect.
     */
    public static long parseLongSafe(final String str) {
        try {
            return Long.parseLong(str);
        } catch (final NumberFormatException e) {
            return 0L;
        }
    }

    public static int parseIntSafe(final String str) {
        try {
            return Integer.parseInt(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    public static short parseShortSafe(final String str) {
        try {
            return Short.parseShort(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    /**
     * @return A capitalized string or "" if the string is empty
     */
    public static String ucFirst(final String str) {
        if (null == str || str.isEmpty()) {
            return "";
        }

        return str.substring(0, 1).toUpperCase() + str.substring(1);
    }

}
