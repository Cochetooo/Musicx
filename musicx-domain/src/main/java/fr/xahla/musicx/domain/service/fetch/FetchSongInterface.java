package fr.xahla.musicx.domain.service.fetch;

import fr.xahla.musicx.api.model.SongInterface;

public interface FetchSongInterface {

    void fetchSongData(final SongInterface song, final boolean overwrite);

}
