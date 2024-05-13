package fr.xahla.musicx.desktop;

import atlantafx.base.theme.PrimerDark;
import fr.xahla.musicx.domain.helper.enums.ApplicationInfo;
import fr.xahla.musicx.desktop.helper.DurationHelper;
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
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/** <b>Main class for the desktop application.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
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
            this.mainStage.setTitle(ApplicationInfo.APP_NAME.getInfo() + " " + ApplicationInfo.APP_VERSION.getInfo());
            this.mainStage.initStyle(StageStyle.DECORATED);
            this.mainStage.setScene(scene);
            this.mainStage.setOnCloseRequest((event) -> Platform.exit());

            DurationHelper.benchmarkFrom("Program Initialization", startTime);

            this.mainStage.show();
        } catch (final IOException exception) {
            logger().log(Level.SEVERE, "Something went wrong during JavaFX initialization.", exception);
        }
    }

}
