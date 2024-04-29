package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.model.SongInterface;

public interface ExternalFetchRepositoryInterface {

    interface AlbumFetcher {
        void fetchAlbumFromExternal(final AlbumInterface album, final boolean overwrite);
    }

    interface ArtistFetcher {
        void fetchArtistFromExternal(final ArtistInterface artist, final boolean overwrite);
    }

    interface SongFetcher {
        void fetchSongFromExternal(final SongInterface song, final boolean overwrite);
    }

}
