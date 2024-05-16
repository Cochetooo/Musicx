package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.helper.FxmlHelper;
import fr.xahla.musicx.desktop.helper.FxmlComponent;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.Parent;

import java.util.ResourceBundle;

/**
 * Manages what is shown on the right side of the application.
 * @author Cochetooo
 * @since 0.3.1
 */
public class RightNavContentManager {

    private final ObjectProperty<Parent> rightNavContent;

    public RightNavContentManager() {
        this.rightNavContent = new SimpleObjectProperty<>();
    }

    /**
     * @since 0.3.1
     */
    public void close() {
        this.rightNavContent.set(null);
    }

    /**
     * @since 0.3.1
     */
    public void switchContent(final FxmlComponent fxmlFile, final ResourceBundle resourceBundle) {
        final var view = FxmlHelper.getComponent(fxmlFile.getFilepath(), resourceBundle);

        rightNavContent.set(null);
        rightNavContent.set(view);
    }

    /**
     * @since 0.3.1
     */
    public Parent get() {
        return rightNavContent.get();
    }

    // --- Listeners ---

    /**
     * @since 0.3.1
     */
    public void onChange(final RightNavContentChangeListener listener) {
        rightNavContent.addListener((observable, oldValue, newValue) -> listener.onChange(oldValue, newValue));
    }

    // --- Inner Classes ---

    /**
     * @since 0.3.1
     */
    public interface RightNavContentChangeListener {
        void onChange(final Parent oldContent, final Parent newContent);
    }
}
