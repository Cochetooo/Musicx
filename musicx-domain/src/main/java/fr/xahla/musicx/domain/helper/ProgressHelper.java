package fr.xahla.musicx.domain.helper;

/** <b>Utility class for progress displaying.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 * @see javafx.scene.control.ProgressBar
 */
public final class ProgressHelper {

    public static double getPercentageFromDouble(
        final double value,
        final double decimal
    ) {
        double percentage = (int) (value * 100 * Math.pow(10, decimal));
        percentage /= (Math.pow(10, decimal));

        return percentage;
    }

}
