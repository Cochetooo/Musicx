package fr.xahla.musicx;

import atlantafx.base.theme.PrimerDark;
import fr.xahla.musicx.desktop.application.MusicxApp;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.config.internationalization.Translator;
import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.util.Locale;

/**
 *
 */
public class Musicx extends Application {

    private static Musicx musicxInstance;

    private MusicxViewController controller;
    private MusicxApp app;

    public static void main(String[] args) {
        Translator.setLocale(new Locale.Builder().setLanguage("en").setRegion("GB").build());

        launch();
    }

    @Override
    public void start(Stage stage) throws Exception {
        musicxInstance = this;

        this.app = new MusicxApp();

        Application.setUserAgentStylesheet(new PrimerDark().getUserAgentStylesheet());

        var fxmlLoader = new FXMLLoader(Musicx.class.getResource("views/musicx.fxml"));
        var root = (Parent) fxmlLoader.load();

        var scene = new Scene(root, 1280, 720);
        stage.setScene(scene);

        this.controller = fxmlLoader.getController();

        this.app.initStage(stage);

        stage.show();
    }

    public MusicxViewController getMainController() {
        return this.controller;
    }

    public MusicxApp getApp() {
        return this.app;
    }

    public static Musicx getInstance() {
        return musicxInstance;
    }
}
