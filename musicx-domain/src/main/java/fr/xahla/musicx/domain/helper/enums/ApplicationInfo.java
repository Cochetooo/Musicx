package fr.xahla.musicx.domain.helper.enums;

/**
 * Constants values of the project information.
 * @author Cochetooo
 * @since 0.2.0
 */
public enum ApplicationInfo {

    APP_AUTHOR("Alexis Cochet"),
    APP_NAME("Musicx"),
    APP_ORGANIZATION("fr.xahla"),
    APP_VERSION("0.5.0");

    private final String info;

    ApplicationInfo(final String info) {
        this.info = info;
    }

    public final String getInfo() {
        return info;
    }

}
