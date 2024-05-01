package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;

import java.util.List;

public interface SongListManagerInterface {

    List<? extends SongDto> getSongs();
    SongRepositoryInterface getRepository();

    void set(final List<? extends SongDto> songs);

}
