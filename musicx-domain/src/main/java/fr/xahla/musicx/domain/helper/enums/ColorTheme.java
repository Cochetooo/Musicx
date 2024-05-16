package fr.xahla.musicx.domain.helper.enums;

import lombok.Getter;

/**
 * List of all colors used for the app theme.
 * @author Cochetooo
 * @since 0.2.3
 */
@Getter
public enum ColorTheme {

    PRIMARY("#e9ffff"),
    GRAY("#808080"),

    SECONDARY("#b2d8d8"),
    ALTERNATIVE("#66b2b2"),
    TERNARY("#008080"),

    BACKGROUND("#006666"),
    ALTERNATIVE_BACKGROUND("#004c4c");

    private String hex;

    ColorTheme(final String hex) {
        this.hex = hex;
    }

}
