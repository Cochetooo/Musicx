package fr.xahla.musicx.desktop.application;

import fr.xahla.musicx.config.db.HibernateLoader;
import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.desktop.application.logging.DesktopStream;
import javafx.stage.Stage;
import org.hibernate.SessionFactory;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;
import java.util.Objects;
import java.util.logging.*;

public class MusicxApp {

    private final HibernateLoader hibernateLoader;
    private final Logger logger;
    private Stage stage;
    private PrintStream printStream;

    public MusicxApp() {
        this.logger = Logger.getLogger(MusicxApp.class.getName());
        this.setupLogger();

        this.hibernateLoader = new HibernateLoader();
    }

    private void setupLogger() {
        try (var inputStream = Objects.requireNonNull(Musicx.class.getResource("config/logger.properties")).openStream()) {
            LogManager.getLogManager().readConfiguration(inputStream);
        } catch (SecurityException | IOException | NullPointerException exception) {
            exception.printStackTrace();
        }

        try (var desktopStream = new DesktopStream()) {
            this.printStream = new PrintStream(desktopStream, true);
            System.setOut(this.printStream);
            System.setErr(this.printStream);

            this.logger.addHandler(new StreamHandler(this.printStream, new SimpleFormatter()));
        } catch (IOException exception) {
            exception.printStackTrace();
        }
    }

    public void initStage(final Stage mainStage) {
        mainStage.setMaximized(true);
        mainStage.setTitle("Musicx 0.1.0");
        mainStage.setOnCloseRequest(windowEvent -> this.dispose());

        this.stage = mainStage;
    }

    public void dispose() {

    }

    public Logger getLogger() {
        return this.logger;
    }

    public PrintStream getPrintStream() {
        return this.printStream;
    }

    public SessionFactory getSessionFactory() {
        return this.hibernateLoader.getSessionFactory();
    }

    public Stage getStage() {
        return this.stage;
    }

}
