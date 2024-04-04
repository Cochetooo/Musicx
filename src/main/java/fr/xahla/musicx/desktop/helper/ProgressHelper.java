package fr.xahla.musicx.desktop.helper;

public class ProgressHelper {

    public static double getPercentageFromDouble(
        final double value,
        final double decimal
    ) {
        double percentage = (int) (value * 100 * Math.pow(10, decimal));
        percentage /= (Math.pow(10, decimal));

        return percentage;
    }

}
