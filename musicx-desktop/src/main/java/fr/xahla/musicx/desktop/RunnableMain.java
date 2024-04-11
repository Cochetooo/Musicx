package fr.xahla.musicx.desktop;

/** <b>This class is made to prevent the executable JAR to give the famous error:
 * "Error: JavaFX runtime components are missing, and are required to run this application"
 * by instantiating a new Main class that does not inherit from <i>Application</i>,
 * causing the application to crash while trying to find javafx modules.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class RunnableMain {

    public static void main(final String[] args) {
        DesktopApplication.main(args);
    }

}
