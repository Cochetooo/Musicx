package fr.xahla.musicx.domain.helper.enums;

/** <b>Enumerates all constant information about a Musicx Application</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public enum ApplicationInfo {

    APP_AUTHOR("Alexis Cochet"),
    APP_NAME("Musicx"),
    APP_ORGANIZATION("fr.xahla"),
    APP_VERSION("0.3.0");

    private final String info;

    ApplicationInfo(final String info) {
        this.info = info;
    }

    public final String getInfo() {
        return info;
    }

}
