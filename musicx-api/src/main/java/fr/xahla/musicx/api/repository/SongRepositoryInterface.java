package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.SongInterface;

/** <b>(API) Interface for Song Repository contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface SongRepositoryInterface {

    void save(SongInterface song);

}
