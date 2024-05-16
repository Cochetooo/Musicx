package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;

import java.util.List;

/**
 * Transferring genre data throughout application layers.
 * @author Cochetooo
 * @since 0.3.0
 */
@Data
@Builder
public class GenreDto {

    private Long id;

    private List<Long> parentIds;

    private String name;

}
