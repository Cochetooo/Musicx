package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;

import java.util.List;

public interface AlbumListManagerInterface {

    List<? extends AlbumInterface> get();
    AlbumRepositoryInterface getRepository();

    void set(final List<? extends AlbumInterface> albums);

}
