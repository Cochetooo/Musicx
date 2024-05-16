package fr.xahla.musicx.domain.helper.enums;

import lombok.Getter;

/**
 * List of all font policies for the app theme.
 * @author Cochetooo
 */
@Getter
public enum FontTheme {

    CONSOLE_FONT("JetBrains Mono"),
    PRIMARY_FONT("Space Grotesk");

    private final String font;

    FontTheme(final String font) {
        this.font = font;
    }

}
