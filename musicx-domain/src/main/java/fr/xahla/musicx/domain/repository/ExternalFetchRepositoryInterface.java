package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.model.data.AlbumInterface;
import fr.xahla.musicx.api.model.data.ArtistInterface;
import fr.xahla.musicx.api.model.data.SongInterface;

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
