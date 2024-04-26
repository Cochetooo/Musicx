/** <b>Domain Module for Musicx apps, should be accessible for anyone that wants to make their own app
 * out of the raw contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
module fr.xahla.musicx.domain {

    // ------------- [REQUIRE] -------------

    // Musicx -> API
    requires fr.xahla.musicx.api;

    // Java -> Logging
    requires java.logging;
    // Java -> Net
    requires java.net.http;

    // Lib -> Dotenv
    requires io.github.cdimascio.dotenv.java;

    // Lib -> JSON
    requires org.json;

    // ------------- [EXPORT] -------------
    exports fr.xahla.musicx.domain.application;
    exports fr.xahla.musicx.domain.helper;
    exports fr.xahla.musicx.domain.manager;
    exports fr.xahla.musicx.domain.model;
    exports fr.xahla.musicx.domain.model.enums;
    exports fr.xahla.musicx.domain.repository;
    exports fr.xahla.musicx.domain.model.data;
    exports fr.xahla.musicx.domain.repository.data;

}