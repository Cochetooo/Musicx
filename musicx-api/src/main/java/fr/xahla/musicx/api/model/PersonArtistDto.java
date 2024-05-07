package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.time.LocalDate;
import java.util.List;

@EqualsAndHashCode(callSuper = true)
@Data
@Builder
public class PersonArtistDto extends ArtistDto {

    private List<Long> bandIds;

    private String firstName;
    private LocalDate birthDate;
    private LocalDate deathDate;

}
