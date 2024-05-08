package fr.xahla.musicx.domain.model.data;

/** <b>Interface defining getters for a Progress task</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface TaskProgressInterface {

    /* @var String name */

    String getName();

    /* @var Double currentProgress */

    double getCurrentProgress();

    /* @var Double totalProgress */

    double getTotalProgress();
}
