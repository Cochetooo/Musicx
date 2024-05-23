package fr.xahla.musicx.desktop.helper;

import fr.xahla.musicx.domain.helper.enums.FontTheme;
import javafx.scene.layout.VBox;
import javafx.scene.text.*;

/**
 * Helper class for text, labels, etc.
 * @author Cochetooo
 * @since 0.4.0
 */
public final class TextHelper {

    /**
     * @since 0.4.0
     */
    public static VBox caption(
        final String primaryStr,
        final String secondaryStr,
        final int primaryFontSize,
        final int secondaryFontSize,
        final Double width,
        final TextAlignment alignment
    ) {
        final var captionBox = new VBox();

        final var primaryText = new Text(primaryStr);
        primaryText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.BOLD, primaryFontSize));
        primaryText.setFill(ColorHelper.PRIMARY);

        final var secondaryText = new Text(secondaryStr);
        secondaryText.setFont(Font.font(FontTheme.PRIMARY_FONT.getFont(), FontWeight.LIGHT, secondaryFontSize));
        secondaryText.setFill(ColorHelper.SECONDARY);

        if (null == width) {
            captionBox.getChildren().setAll(primaryText, secondaryText);
        } else {
            final var textFlow = new TextFlow();
            textFlow.setMaxWidth(width);
            textFlow.setTextAlignment(alignment);
            textFlow.getChildren().addAll(primaryText, secondaryText);
            captionBox.getChildren().setAll(textFlow);
        }

        return captionBox;
    }

}
