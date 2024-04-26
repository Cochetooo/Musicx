package fr.xahla.musicx.domain.helper;

public enum FontTheme {

    PRIMARY_FONT("Space Grotesk");

    private String font;

    FontTheme(final String font) {
        this.font = font;
    }

    public final String getFont() {
        return font;
    }

}
