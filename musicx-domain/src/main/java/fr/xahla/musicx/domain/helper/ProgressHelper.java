package fr.xahla.musicx.domain.helper;

/**
 * Utility class for Progress bars.
 * @author Cochetooo
 */
public final class ProgressHelper {

    /**
     * Converts a double as a percentage.
     * @param value The initial double value.
     * @param decimal The number of decimal of the percentage.
     */
    public static double progress_get_percentage(
        final double value,
        final double decimal
    ) {
        double percentage = (int) (value * 100 * Math.pow(10, decimal));
        percentage /= (Math.pow(10, decimal));

        return percentage;
    }

}
