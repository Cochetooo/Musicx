package fr.xahla.musicx.infrastructure.model.data.enums;

/** <b>Enumerates all constant information about Musicx</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public enum SoftwareInfo {

    APP_NAME("Musicx"),
    APP_VERSION("0.2.4"),

    APP_PRIMARY_FONT("Space Grotesk"),

    LASTFM_TOKEN("f17c4b2c645d912c0f222eaa77f58751");

    private String info;

    SoftwareInfo(final String info) {
        this.info = info;
    }

    public String getInfo() {
        return info;
    }

}
