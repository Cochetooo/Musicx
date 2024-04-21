package fr.xahla.musicx.core.config;

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
