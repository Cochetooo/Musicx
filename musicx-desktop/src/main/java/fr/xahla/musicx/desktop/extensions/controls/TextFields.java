package fr.xahla.musicx.desktop.extensions.controls;

import javafx.scene.control.TextField;
import javafx.scene.control.TextFormatter;

import java.util.function.UnaryOperator;

/**
 * Extension class for JavaFX TextField components
 *
 * @author Cochetooo
 * @since 0.5.1
 */
public final class TextFields {

    /**
     * Set a constraint of integer-only to the TextField.
     * @since 0.5.1
     */
    public static void setIntegerTextFieldConstraint(final TextField textField) {
        final UnaryOperator<TextFormatter.Change> filter = change -> {
            final var newText = change.getControlNewText();

            if (newText.matches("\\d*")) {
                return change;
            }

            return null;
        };

        textField.setTextFormatter(new TextFormatter<>(filter));
    }

}
