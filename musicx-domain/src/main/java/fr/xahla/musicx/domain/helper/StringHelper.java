package fr.xahla.musicx.domain.helper;

/**
 * Utility class for String
 * @author Cochetooo
 * @since 0.3.0
 */
public final class StringHelper {

    /**
     * @return The parsed long or 0 if the format is incorrect.
     * @since 0.3.0
     */
    public static long str_parse_long_safe(final String str) {
        try {
            return Long.parseLong(str);
        } catch (final NumberFormatException e) {
            return 0L;
        }
    }

    /**
     * @since 0.3.0
     */
    public static int str_parse_int_safe(final String str) {
        try {
            return Integer.parseInt(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    /**
     * @since 0.3.0
     */
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
     * @since 0.3.1
     */
    public static String str_uc_first(final String str) {
        if (null == str || str.isEmpty()) {
            return "";
        }

        return str.substring(0, 1).toUpperCase() + str.substring(1);
    }

    final private static String invalidChars = "[\\\\/:*?\"<>|]";

    /**
     * @return The same string without any of those characters : [\\/:*?"&lt;&gt;|] to fit a valid Path
     * @since 0.5.0
     */
    public static String str_convert_to_path(final String str) {
        return str.replaceAll(invalidChars, "");
    }

    /**
     * @return True if the needle is contained in the haystack string, ignoring case.
     * @since 0.5.0
     */
    public static boolean str_contains_ignore_case(final String haystack, final String needle) {
        return haystack.toLowerCase().contains(needle.toLowerCase());
    }

}
