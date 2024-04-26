package fr.xahla.musicx.domain.helper;

/** <b>List of main colors to be used in applications.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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

    public String getHex() {
        return hex;
    }

}
