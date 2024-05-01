package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;

import java.util.List;

public interface ArtistListManagerInterface {

    List<? extends ArtistDto> get();
    ArtistRepositoryInterface getRepository();

    void set(final List<? extends ArtistDto> artists);

}
