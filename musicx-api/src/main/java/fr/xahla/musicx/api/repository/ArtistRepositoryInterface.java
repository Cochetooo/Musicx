package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.ArtistInterface;

public interface ArtistRepositoryInterface {

    ArtistInterface findById(final Integer id);

    void save(final ArtistInterface artist);

}
