package fr.xahla.musicx.domain.service.fetch;

import fr.xahla.musicx.api.model.ArtistInterface;

public interface FetchArtistInterface {

    void fetchArtistData(final ArtistInterface artist, final boolean overwrite);

}
