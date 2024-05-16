package fr.xahla.musicx.domain.helper;

/**
 * Utility class for String
 * @author Cochetooo
 */
public final class StringHelper {

    /**
     * @return The parsed long or 0 if the format is incorrect.
     */
    public static long str_parse_long_safe(final String str) {
        try {
            return Long.parseLong(str);
        } catch (final NumberFormatException e) {
            return 0L;
        }
    }

    public static int str_parse_int_safe(final String str) {
        try {
            return Integer.parseInt(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    public static short str_parse_short_safe(final String str) {
        try {
            return Short.parseShort(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    public static boolean str_is_null_or_blank(final String str) {
        return null == str || str.isBlank();
    }

    /**
     * @return A capitalized string or "" if the string is empty
     */
    public static String str_uc_first(final String str) {
        if (null == str || str.isEmpty()) {
            return "";
        }

        return str.substring(0, 1).toUpperCase() + str.substring(1);
    }

}
