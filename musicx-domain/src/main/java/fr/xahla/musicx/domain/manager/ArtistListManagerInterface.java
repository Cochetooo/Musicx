package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;

import java.util.List;

public interface ArtistListManagerInterface {

    List<? extends ArtistInterface> get();
    ArtistRepositoryInterface getRepository();

    void set(final List<? extends ArtistInterface> artists);

}
