package fr.xahla.musicx.core.logging;

import java.util.logging.Logger;

/** <b>Class that stores a logger for the application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class SimpleLogger {

    private static Logger logger;

    public static void setLogger(final Logger _logger) {
        logger = _logger;
    }

    public static Logger logger() {
        return logger;
    }

}
