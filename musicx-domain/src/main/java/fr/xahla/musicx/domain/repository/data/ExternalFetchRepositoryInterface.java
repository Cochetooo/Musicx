package fr.xahla.musicx.domain.repository.data;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.model.SongDto;

import java.util.List;

/**
 * For each external APIs, defines contracts that can be used by those APIs.
 * @author Cochetooo
 * @since 0.3.0
 */
public interface ExternalFetchRepositoryInterface {

    /**
     * API that handles album data fetching.
     * @since 0.3.0
     */
    interface AlbumFetcher {
        void fetchAlbumFromExternal(final AlbumDto album, final boolean overwrite);
    }

    /**
     * API that handles album search.
     * @since 0.3.2
     */
    interface AlbumSearch {
        List<AlbumDto> searchAlbum(final String term);
    }

    /**
     * API that handles artist data fetching.
     * @since 0.3.0
     */
    interface ArtistFetcher {
        void fetchArtistFromExternal(final ArtistDto artist, final boolean overwrite);
    }

    /**
     * API that handles artist search.
     * @since 0.3.2
     */
    interface ArtistSearch {
        List<ArtistDto> searchArtist(final String term);
    }

    /**
     * API that handles label search.
     * @since 0.3.2
     */
    interface LabelSearch {
        List<LabelDto> searchLabel(final String term);
    }

    /**
     * API that handles song data fetching.
     * @since 0.3.0
     */
    interface SongFetcher {
        void fetchSongFromExternal(final SongDto song, final boolean overwrite);
    }

    /**
     * API that handles song search.
     * @since 0.3.2
     */
    interface SongSearch {
        List<SongDto> searchSong(final String term);
    }

}
