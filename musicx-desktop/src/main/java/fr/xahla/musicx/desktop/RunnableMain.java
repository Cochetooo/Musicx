package fr.xahla.musicx.desktop;

/**
 * This class is made to prevent the executable JAR to give the famous error:
 * "Error: JavaFX runtime components are missing, and are required to run this application"
 * by instantiating a new Main class that does not inherit from <b>Application</b>,
 * causing the application to crash while trying to find javafx modules.
 */
public class RunnableMain {

    public static void main(final String[] args) {
        DesktopApplication.main(args);
    }

}
