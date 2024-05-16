package fr.xahla.musicx.domain.repository.data;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.model.SongDto;

import java.util.List;

/**
 * For each external APIs, defines contracts that can be used by those APIs.
 * @author Cochetooo
 */
public interface ExternalFetchRepositoryInterface {

    /**
     * API that handles album data fetching.
     */
    interface AlbumFetcher {
        void fetchAlbumFromExternal(final AlbumDto album, final boolean overwrite);
    }

    /**
     * API that handles album search.
     */
    interface AlbumSearch {
        List<AlbumDto> searchAlbum(final String term);
    }

    /**
     * API that handles artist data fetching.
     */
    interface ArtistFetcher {
        void fetchArtistFromExternal(final ArtistDto artist, final boolean overwrite);
    }

    /**
     * API that handles artist search.
     */
    interface ArtistSearch {
        List<ArtistDto> searchArtist(final String term);
    }

    /**
     * API that handles label search.
     */
    interface LabelSearch {
        List<LabelDto> searchLabel(final String term);
    }

    /**
     * API that handles song data fetching.
     */
    interface SongFetcher {
        void fetchSongFromExternal(final SongDto song, final boolean overwrite);
    }

    /**
     * API that handles song search.
     */
    interface SongSearch {
        List<SongDto> searchSong(final String term);
    }

}
