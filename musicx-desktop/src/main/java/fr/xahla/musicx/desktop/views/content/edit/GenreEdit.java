package fr.xahla.musicx.desktop.views.content.edit;

import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.scene;

/**
 * View for genre editing.
 * @author Cochetooo
 * @since 0.3.3
 */
public class GenreEdit implements Initializable {

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {

    }

    public void close() {
        scene().getRightNavContent().close();
    }

}
