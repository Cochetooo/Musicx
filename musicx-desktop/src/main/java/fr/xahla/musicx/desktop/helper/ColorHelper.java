package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.domain.helper.enums.ColorTheme;
import javafx.scene.Node;
import javafx.scene.paint.Color;

/**
 * Utility class for JavaFX Colors, containing color theme cast for JavaFX.
 * @author Cochetooo
 * @since 0.2.3
 */
public class ColorHelper {

    public static final Color PRIMARY = Color.web(ColorTheme.PRIMARY.getHex());
    public static final Color GRAY = Color.web(ColorTheme.GRAY.getHex());
    public static final Color SECONDARY = Color.web(ColorTheme.SECONDARY.getHex());
    public static final Color TERNARY = Color.web(ColorTheme.TERNARY.getHex());
    public static final Color BACKGROUND = Color.web(ColorTheme.BACKGROUND.getHex());
    public static final Color ALTERNATIVE = Color.web(ColorTheme.ALTERNATIVE.getHex());
    public static final Color ALTERNATIVE_BACKGROUND = Color.web(ColorTheme.ALTERNATIVE_BACKGROUND.getHex());

    /**
     * @since 0.4.0
     */
    public static void backgroundColor(final Node node, final String color) {
        node.setStyle("-fx-background-color: -musicx-color-" + color + ";");
    }

}
