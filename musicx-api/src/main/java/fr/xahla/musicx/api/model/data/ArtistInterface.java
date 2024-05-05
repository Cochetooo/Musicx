package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.BandArtistDto;

import java.util.List;
import java.util.Locale;

public interface ArtistInterface {

    /* --- PRIMARY KEY --- */

    Long getId();
    ArtistInterface setId(final Long id);

    /* --- DTO Cast --- */

    ArtistInterface fromDto(final ArtistDto artistDto);
    ArtistDto toDto();

    /* --- Albums --- */

    List<? extends AlbumInterface> getAlbums();
    ArtistInterface setAlbums(final List<? extends AlbumInterface> albums);

    /* --- Artwork URL --- */

    String getArtworkUrl();
    ArtistInterface setArtworkUrl(final String artworkUrl);

    /* --- Country --- */

    Locale getCountry();
    ArtistInterface setCountry(final Locale country);

    /* --- Name --- */

    String getName();
    ArtistInterface setName(final String name);

    /* --- Songs --- */

    List<? extends SongInterface> getSongs();
    ArtistInterface setSongs(final List<? extends SongInterface> songs);
}
