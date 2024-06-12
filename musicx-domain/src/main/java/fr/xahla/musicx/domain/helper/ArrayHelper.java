package fr.xahla.musicx.domain.helper;

import java.util.List;

/**
 * Utility class for arrays.
 * @author Cochetooo
 * @since 0.1.0
 */
public final class ArrayHelper {

    /**
     * Checks if a value exists in an array.
     *
     * @param needle The searched value.
     * @param haystack The array.
     * @return <b>true</b> if needle is found in the array, <b>false</b> otherwise.
     * @since 0.1.0
     */
    public static boolean array_in(final Object needle, final Object[] haystack) {
        for (final var obj : haystack) {
            if (obj.equals(needle)) {
                return true;
            }
        }

        return false;
    }

    /**
     * Checks if a string exists in a string array, case-insensitive.
     *
     * @param needle The searched value.
     * @param haystack The array.
     * @return <b>true</b> if needle is found in the array, <b>false</b> otherwise.
     * @since 0.1.0
     */
    public static boolean array_in_string_ignore_case(final String needle, final List<String> haystack) {
        for (final var string : haystack) {
            if (string.equalsIgnoreCase(needle)) {
                return true;
            }
        }

        return false;
    }

    /**
     * Checks if a string is contained in a string array element, case-insensitive.
     *
     * @param needle The searched value.
     * @param haystack The array.
     * @return <b>true</b> if needle is found in an array element, <b>false</b> otherwise.
     * @since 0.5.0
     */
    public static boolean array_contains_string_ignore_case(final String needle, final List<String> haystack) {
        for (final var string : haystack) {
            if (string.toLowerCase().contains(needle.toLowerCase())) {
                return true;
            }
        }

        return false;
    }

}
