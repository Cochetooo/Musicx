package fr.xahla.musicx.desktop;

import atlantafx.base.theme.PrimerDark;
import fr.xahla.musicx.core.config.AppInterface;
import fr.xahla.musicx.core.config.HibernateLoader;
import fr.xahla.musicx.core.logging.SimpleLogger;
import fr.xahla.musicx.desktop.logging.DesktopStream;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

import java.io.IOException;
import java.io.PrintStream;
import java.util.Locale;
import java.util.Objects;
import java.util.ResourceBundle;
import java.util.logging.*;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public final class DesktopApplication extends Application implements AppInterface {

    private static DesktopApplication appInstance;

    private PrintStream printStream;

    private Stage mainStage;

    public static void main(final String[] args) {
        launch();
    }

    @Override public void start(final Stage stage) {
        appInstance = this;

        this.setupLogger();
        this.setupDatabase();

        this.mainStage = stage;

        this.setupApp();
    }

    @Override public void setupApp() {
        final var startTime = System.currentTimeMillis();

        DesktopContext.createContext();

        Locale.setDefault(Locale.ENGLISH);
        Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());

        final var resources = ResourceBundle.getBundle("fr.xahla.musicx.desktop.translations.messages");
        final var fxmlLoader = new FXMLLoader(DesktopApplication.class.getResource("views/application.fxml"), resources);

        try {
            final var root = (Parent) fxmlLoader.load();

            final var scene = new Scene(root, 1280, 720);

            // Font Family
            scene.getStylesheets().add(
                "https://fonts.googleapis.com/css2?family=Montserrat:ital,wght@0,100..900;1,100..900&display=swap"
            );

            scene.getStylesheets().add(
                "https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@300..700&display=swap"
            );

            // Additional CSS
            scene.getStylesheets().add(
                Objects.requireNonNull(DesktopApplication.class.getResource("assets/app.css")).toExternalForm()
            );

            this.mainStage.setMaximized(true);
            this.mainStage.setTitle("Musicx 0.2.0");
            this.mainStage.setScene(scene);

            logger().config("Initialization time: " + (System.currentTimeMillis() - startTime) + "ms.");

            this.mainStage.show();
        } catch (IOException e) {
            logger().severe(e.getLocalizedMessage());
            e.printStackTrace();
        }
    }

    @Override public void setupLogger() {
        SimpleLogger.setLogger(Logger.getLogger(DesktopApplication.class.getName()));

        try (var inputStream = Objects.requireNonNull(DesktopApplication.class.getResource("config/logger.properties")).openStream()) {
            LogManager.getLogManager().readConfiguration(inputStream);
        } catch (SecurityException | IOException | NullPointerException exception) {
            logger().severe(exception.getLocalizedMessage());
        }

        logger().setLevel(Level.ALL);

        try (var desktopStream = new DesktopStream()) {
            this.printStream = new PrintStream(desktopStream, true);
            System.setOut(this.printStream);
            System.setErr(this.printStream);

            logger().addHandler(new StreamHandler(this.printStream, new SimpleFormatter()));
        } catch (IOException exception) {
            logger().severe(exception.getLocalizedMessage());
        }
    }

    @Override public void setupDatabase() {
        HibernateLoader.setHibernateLoader(new HibernateLoader());
    }

}
