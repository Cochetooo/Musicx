package fr.xahla.musicx.domain.helper.enums;

/**
 * Constants values of the project information.
 * @author Cochetooo
 */
public enum ApplicationInfo {

    APP_AUTHOR("Alexis Cochet"),
    APP_NAME("Musicx"),
    APP_ORGANIZATION("fr.xahla"),
    APP_VERSION("0.3.2");

    private final String info;

    ApplicationInfo(final String info) {
        this.info = info;
    }

    public final String getInfo() {
        return info;
    }

}
