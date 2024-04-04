package fr.xahla.musicx.desktop.views.footer.appInfo;

import fr.xahla.musicx.desktop.helper.ProgressHelper;
import fr.xahla.musicx.desktop.manager.LibraryViewManager;
import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.desktop.model.SongViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.config.internationalization.Translator;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import javafx.application.Platform;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.collections.ListChangeListener;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.layout.HBox;

import java.time.Duration;
import java.util.concurrent.atomic.AtomicInteger;

public class AppInfoViewController implements ViewControllerInterface {

    public record Props(
        LibraryViewManager libraryManager
    ) implements ViewControllerProps {}

    @FXML public HBox footerLeftBox;
    @FXML public HBox footerRightBox;
    @FXML public Label footerLibrarySumLabel;

    private MusicxViewController parent;
    private AppInfoViewController.Props props;

    private ProgressBar progressBar;
    private Label progressLabel;
    private Label progressStatusLabel;

    private Translator translator;
    private String footerLibrarySumPartialText;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;
        this.props = (AppInfoViewController.Props) props;

        this.translator = new Translator("footer");
        this.makeTranslations();

        this.refreshLibraryInfo();
        this.props.libraryManager().getLibrary().songsProperty().addListener((ListChangeListener<SongViewModel>) change -> Platform.runLater(this::refreshLibraryInfo));
    }

    @Override
    public MusicxViewController getParent() {
        return this.parent;
    }

    @Override public void makeTranslations() {
        this.footerLibrarySumPartialText = this.translator.translate("librarySum");
    }

    public void invokeProgressBar(Task<Object> task, String text) {
        // Clear in case a progress bar was already there
        this.revokeProgressBar();

        this.progressBar = new ProgressBar();
        this.progressBar.setPrefHeight(20);
        this.progressBar.progressProperty().bind(task.progressProperty());
        this.progressLabel = new Label(text);
        this.progressStatusLabel = new Label("--.-%");
        this.progressBar.progressProperty().addListener((observableValue, oldValue, newValue) -> {
            this.progressStatusLabel.setText(ProgressHelper.getPercentageFromDouble(newValue.doubleValue(), 1) + "%");
        });

        this.footerLeftBox.getChildren().addAll(this.progressLabel, this.progressBar, this.progressStatusLabel);
        new Thread(task).start();
    }

    public void revokeProgressBar() {
        if (null != this.progressLabel && null != this.progressBar) {
            this.footerLeftBox.getChildren().removeAll(this.progressLabel, this.progressBar, this.progressStatusLabel);
        }
    }

    public void refreshLibraryInfo() {
        var totalDuration = new AtomicInteger(0);

        var library = this.props.libraryManager().getLibrary();

        library.getSongs().forEach((song) -> {
            totalDuration.getAndAdd(song.getDuration());
        });

        this.footerLibrarySumLabel.setText(
            library.getName() + " - " +
            library.getSongs().size() + " " + this.footerLibrarySumPartialText +
                DurationHelper.getDurationFormatted(Duration.ofSeconds(totalDuration.get()))
        );
    }
}
