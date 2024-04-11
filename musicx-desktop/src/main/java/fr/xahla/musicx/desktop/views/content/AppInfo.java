package fr.xahla.musicx.desktop.views.content;

import fr.xahla.musicx.desktop.helper.DurationHelper;
import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Label;

import java.net.URL;
import java.time.Duration;
import java.util.ResourceBundle;
import java.util.concurrent.atomic.AtomicLong;

import static fr.xahla.musicx.desktop.DesktopContext.library;

/** <b>View for the little row showing app informations at the bottom.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class AppInfo implements Initializable {

    @FXML private Label footerLibrarySumLabel;

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;

        this.updateLibraryInfo();
        library().onSongsChange(change -> Platform.runLater(this::updateLibraryInfo));
    }

    private void updateLibraryInfo() {
        final var totalDuration = new AtomicLong(0L);

        library().getSongs().forEach((song) -> {
            totalDuration.getAndAdd(song.getDuration());
        });

        this.footerLibrarySumLabel.setText(
            library().getName() + " - "
            + library().getSongs().size() + " " + this.resourceBundle.getString("appInfo.librarySum")
            + " " + DurationHelper.getDurationFormatted(Duration.ofSeconds(totalDuration.get()))
        );
    }
}
