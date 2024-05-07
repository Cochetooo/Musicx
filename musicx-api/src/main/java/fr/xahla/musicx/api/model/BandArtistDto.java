package fr.xahla.musicx.api.model;

import lombok.Builder;
import lombok.Data;
import lombok.EqualsAndHashCode;

import java.util.List;

@EqualsAndHashCode(callSuper = true)
@Data
@Builder
public class BandArtistDto extends ArtistDto {

    private List<Long> memberIds;

}
