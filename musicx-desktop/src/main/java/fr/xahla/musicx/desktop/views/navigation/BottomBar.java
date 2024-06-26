package fr.xahla.musicx.desktop.views.navigation;

import fr.xahla.musicx.domain.helper.DurationHelper;
import fr.xahla.musicx.domain.helper.ProgressHelper;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.layout.HBox;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.concurrent.atomic.AtomicLong;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for the little row showing app information at the bottom.
 * @author Cochetooo
 * @since 0.2.0
 */
public class BottomBar implements Initializable {

    @FXML private HBox footerLeftBox;
    @FXML private Label footerLibrarySumLabel;

    private ResourceBundle resourceBundle;

    private ProgressBar taskProgressBar;
    private Label taskProgressLabel, taskProgressStatusLabel;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.updateLibraryInfo();
        scene().getLocalLibraryScene().getLibrary().onLocalSongsChange(change -> Platform.runLater(this::updateLibraryInfo));

        final var taskProgress = scene().getTaskProgress();

        taskProgress.onTaskProgressChange((observableValue, oldValue, newValue) -> {
            this.revokeProgressBar();

            this.taskProgressBar = new ProgressBar();
            this.taskProgressBar.setPrefHeight(20);
            this.taskProgressBar.progressProperty().bind(taskProgress.getTaskProgress().currentProgressProperty());

            final var translatedTaskProgressLabelText = resourceBundle.getString(newValue.getName());

            this.taskProgressLabel = new Label(translatedTaskProgressLabelText);
            this.taskProgressStatusLabel = new Label("--.-%");

            this.taskProgressBar.progressProperty().addListener(((observableValue1, oldProgress, newProgress) -> {
                this.taskProgressLabel.setText(translatedTaskProgressLabelText + " ("
                    + (int) (taskProgress.getTaskProgress().getCurrentProgress() * taskProgress.getTaskProgress().getTotal()) + "/"
                    + (int) taskProgress.getTaskProgress().getTotal() + ")"
                );
                this.taskProgressStatusLabel.setText(ProgressHelper.progress_get_percentage(newProgress.doubleValue(), 1) + "%");

                if (newProgress.doubleValue() >= 1.0) {
                    this.revokeProgressBar();
                }
            }));

            this.footerLeftBox.getChildren().addAll(this.taskProgressLabel, this.taskProgressBar, this.taskProgressStatusLabel);
            new Thread(newValue.getTask()).start();
        });
    }

    private void revokeProgressBar() {
        if (null != this.taskProgressLabel && null != this.taskProgressBar && null != this.taskProgressStatusLabel) {
            this.footerLeftBox.getChildren().removeAll(
                this.taskProgressBar,
                this.taskProgressLabel,
                this.taskProgressStatusLabel
            );
        }
    }

    private void updateLibraryInfo() {
        final var totalDuration = new AtomicLong(0L);

        final var songs = scene().getLocalLibraryScene().getLibrary().getLocalSongs();

        songs.forEach((song)
            -> totalDuration.getAndAdd(song.getDuration()));

        this.footerLibrarySumLabel.setText(
            songs.size() + " " + this.resourceBundle.getString("appInfo.librarySum")
                + " " + DurationHelper.getTimeString(java.time.Duration.ofMillis(totalDuration.get()))
        );
    }
}
