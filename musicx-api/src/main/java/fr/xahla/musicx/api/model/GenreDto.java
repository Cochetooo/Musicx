package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;

import java.util.List;

/** <b>(API) Interface for Genre Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
@Data
@Builder
public class GenreDto {

    private Long id;

    private List<Long> parentIds;

    private String name;

}
