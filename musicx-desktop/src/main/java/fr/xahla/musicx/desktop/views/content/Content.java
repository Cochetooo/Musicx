package fr.xahla.musicx.desktop.views.content;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.domain.helper.StringHelper;
import fr.xahla.musicx.domain.helper.enums.FontTheme;
import fr.xahla.musicx.desktop.helper.ColorHelper;
import fr.xahla.musicx.desktop.helper.DurationHelper;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.desktop.model.entity.Song;
import javafx.beans.property.SimpleObjectProperty;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TableCell;
import javafx.scene.control.TableColumn;
import javafx.scene.control.TableView;
import javafx.scene.image.ImageView;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;
import javafx.scene.text.Text;
import javafx.util.Duration;

import java.net.URL;
import java.time.LocalDate;
import java.util.ResourceBundle;

import static fr.xahla.musicx.desktop.context.DesktopContext.*;

/**
 * View for the main center content with the track list and the search bar.
 * @author Cochetooo
 * @since 0.2.0
 */
public class Content implements Initializable {

    private ResourceBundle resourceBundle;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        this.resourceBundle = resourceBundle;
    }

    @FXML public void actionSettings() {
        FxmlHelper.showModal(FxmlComponent.MODAL_SETTINGS, this.resourceBundle, resourceBundle.getString("settings.title"));
    }

    @FXML public void actionQueueList() {
        if (null != rightNavContent().get()) {
            rightNavContent().close();
        } else {
            rightNavContent().switchContent(FxmlComponent.QUEUE_LIST, this.resourceBundle);
        }
    }
}
