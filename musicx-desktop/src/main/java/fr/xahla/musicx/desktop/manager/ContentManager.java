package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.helper.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.scene.Parent;

import java.util.ResourceBundle;

/**
 * Manages what is shown on the right side of the application.
 * @author Cochetooo
 * @since 0.3.1
 */
public class ContentManager {

    private final ObjectProperty<Parent> content;

    public ContentManager() {
        this.content = new SimpleObjectProperty<>();
    }

    /**
     * @since 0.3.1
     */
    public void close() {
        this.content.set(null);
    }

    /**
     * @since 0.3.1
     */
    public void switchContent(final FxmlComponent fxmlFile, final ResourceBundle resourceBundle) {
        final var view = FxmlHelper.getComponent(fxmlFile.getFilepath(), resourceBundle);

        content.set(view);
    }

    /**
     * @since 0.3.1
     */
    public Parent get() {
        return content.get();
    }

    // --- Listeners ---

    /**
     * @since 0.3.1
     */
    public void onChange(final ContentChangeListener listener) {
        content.addListener((observable, oldValue, newValue) -> listener.onChange(oldValue, newValue));
    }

    // --- Inner Classes ---

    /**
     * @since 0.3.1
     */
    public interface ContentChangeListener {
        void onChange(final Parent oldContent, final Parent newContent);
    }
}
