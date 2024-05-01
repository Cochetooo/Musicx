package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;

import java.util.List;

public interface AlbumListManagerInterface {

    List<? extends AlbumDto> get();
    AlbumRepositoryInterface getRepository();

    void set(final List<? extends AlbumDto> albums);

}
