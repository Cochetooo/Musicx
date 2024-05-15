package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;

import java.util.List;

/**
 * Transferring label data throughout application layers.
 * @author Cochetooo
 */
@Data
@Builder
public class LabelDto {

    private Long id;

    private List<Long> genreIds;

    private String name;

}
