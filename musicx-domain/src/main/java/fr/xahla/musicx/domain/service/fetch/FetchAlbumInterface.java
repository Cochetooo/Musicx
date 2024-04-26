package fr.xahla.musicx.domain.service.fetch;

import fr.xahla.musicx.api.model.AlbumInterface;

public interface FetchAlbumInterface {

    void fetchAlbumData(final AlbumInterface album, final boolean overwrite);

}
