package fr.xahla.musicx.domain.repository.data;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.SongDto;

public interface ExternalFetchRepositoryInterface {

    interface AlbumFetcher {
        void fetchAlbumFromExternal(final AlbumDto album, final boolean overwrite);
    }

    interface ArtistFetcher {
        void fetchArtistFromExternal(final ArtistDto artist, final boolean overwrite);
    }

    interface SongFetcher {
        void fetchSongFromExternal(final SongDto song, final boolean overwrite);
    }

}
