package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.helper.FXMLHelper;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.Parent;

import java.util.ResourceBundle;

public class RightNavContentManager {

    private final ObjectProperty<Parent> rightNavContent;

    public RightNavContentManager() {
        this.rightNavContent = new SimpleObjectProperty<>();
    }

    public void close() {
        this.rightNavContent.set(null);
    }

    public void switchContent(final FxmlComponent fxmlFile, final ResourceBundle resourceBundle) {
        final var view = FXMLHelper.getComponent(fxmlFile.getFilepath(), resourceBundle);

        rightNavContent.set(null);
        rightNavContent.set(view);
    }

    public Parent get() {
        return rightNavContent.get();
    }

    // --- Listeners ---

    public void onChange(final RightNavContentChangeListener listener) {
        rightNavContent.addListener((observable, oldValue, newValue) -> listener.onChange(oldValue, newValue));
    }

    // --- Inner Classes ---

    public interface RightNavContentChangeListener {
        void onChange(final Parent oldContent, final Parent newContent);
    }
}
