package fr.xahla.musicx.desktop.helper;

/** <b>Utility class for arrays.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class ArrayHelper {

    /**
     * Checks if a value exists in an array.
     *
     * @param needle The searched value.
     * @param haystack The array.
     * @return <b>true</b> if needle is found in the array, <b>false</b> otherwise.
     */
    public static boolean inArray(final Object needle, final Object[] haystack) {
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
     */
    public static boolean inArrayStringIgnoreCase(final String needle, final String[] haystack) {
        for (final var string : haystack) {
            if (string.equalsIgnoreCase(needle)) {
                return true;
            }
        }

        return false;
    }

}
