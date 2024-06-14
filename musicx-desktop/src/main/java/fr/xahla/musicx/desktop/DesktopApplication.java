package fr.xahla.musicx.desktop;

import atlantafx.base.theme.PrimerDark;
import fr.xahla.musicx.desktop.context.DesktopContext;
import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.domain.helper.Benchmark;
import fr.xahla.musicx.domain.helper.enums.ApplicationInfo;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.io.IOException;
import java.util.Locale;
import java.util.Objects;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.config;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Handles JavaFX of the main application.
 * @author Cochetooo
 */
public final class DesktopApplication extends Application {

    private Stage mainStage;

    public static void main(final String[] args) {
        launch();
    }

    @Override public void start(final Stage stage) {
        this.mainStage = stage;

        this.setupApp();
    }

    public void setupApp() {
        final var benchmark = new Benchmark();

        DesktopContext.createContext();

        Locale.setDefault(Locale.of(config().getLanguage()));
        Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());

        final var resources = ResourceBundle.getBundle("fr.xahla.musicx.desktop.translations.messages");
        final var fxmlLoader = new FXMLLoader(DesktopApplication.class.getResource("views/Application.fxml"), resources);

        try {
            final var root = (Parent) fxmlLoader.load();

            final var scene = new Scene(root, 1280, 720);

            // Font Family
            scene.getStylesheets().add(
                "https://fonts.googleapis.com/css2?family=Montserrat:ital,wght@0,100..900;1,100..900&display=swap"
            );

            scene.getStylesheets().add(
                "https://fonts.googleapis.com/css2?family=Orbitron:wght@400..900&display=swap"
            );

            scene.getStylesheets().add(
                "https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@300..700&display=swap"
            );

            // Additional CSS
            scene.getStylesheets().add(
                Objects.requireNonNull(DesktopApplication.class.getResource("assets/app.css")).toExternalForm()
            );

            this.mainStage.setMaximized(true);
            this.mainStage.setTitle(ApplicationInfo.APP_NAME.getInfo() + " " + ApplicationInfo.APP_VERSION.getInfo());
            this.setWindowIcon();
            this.mainStage.initStyle(StageStyle.DECORATED);
            this.mainStage.setScene(scene);
            this.mainStage.setOnCloseRequest((event) -> Platform.exit());

            benchmark.print("Program Initialization");

            this.mainStage.show();
        } catch (final IOException exception) {
            logger().error(exception, "JAVA_FX_INITIALIZATION_ERROR");
        }
    }

    private void setWindowIcon() {
        final var icon = ImageHelper.loadImageFromResource("logo500.png");
        this.mainStage.getIcons().add(icon);
    }

}
