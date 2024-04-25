package fr.xahla.musicx.infrastructure.model.data;

/** <b>This interface defines primary methods to initialize a front-end app.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface AppInterface {

    void setupApp();

    void setupLogger();

    void setupDatabase();

}
