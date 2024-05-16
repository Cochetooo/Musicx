package fr.xahla.musicx.desktop;

/** This class is made to prevent the executable JAR to give the famous error:<br>
 * "Error: JavaFX runtime components are missing, and are required to run this application"
 * by instantiating a new Main class that does not inherit from <i>Application</i>,
 * causing the application to crash while trying to find javafx modules.
 *
 * @author Cochetooo
 * @since 0.2.1
 */
public class RunnableMain {

    public static void main(final String[] args) {
        DesktopApplication.main(args);
    }

}
