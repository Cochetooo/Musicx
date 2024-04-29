package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;

import java.util.List;

public interface SongListManagerInterface {

    List<? extends SongInterface> getSongs();
    SongRepositoryInterface getRepository();

    void set(final List<? extends SongInterface> songs);

}
