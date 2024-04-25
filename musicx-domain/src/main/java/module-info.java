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
    requires java.logging;

    exports fr.xahla.musicx.domain.application;
    exports fr.xahla.musicx.domain.manager;
    exports fr.xahla.musicx.domain.model;
    exports fr.xahla.musicx.domain.repository;

}