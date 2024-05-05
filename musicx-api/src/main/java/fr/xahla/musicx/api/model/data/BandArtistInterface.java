package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.BandArtistDto;

import java.util.List;

public interface BandArtistInterface extends ArtistInterface {

    /* --- Artists --- */

    List<? extends PersonArtistInterface> getMembers();
    BandArtistInterface setMembers(final List<? extends PersonArtistInterface> artists);

    /* --- Related Bands --- */

    List<? extends BandArtistInterface> getRelatedBands();
    BandArtistInterface setRelatedBands(final List<? extends BandArtistInterface> relatedBands);

}
