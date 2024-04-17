package fr.xahla.musicx.core.config;

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
public enum ProjectInfo {

    APP_NAME("Musicx"),
    APP_VERSION("0.2.3");

    private String info;

    ProjectInfo(final String info) {
        this.info = info;
    }

    public String getInfo() {
        return info;
    }

}
