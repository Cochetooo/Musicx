package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.enums.AlbumType;
import fr.xahla.musicx.api.model.enums.ArtistRole;

import java.time.LocalDate;
import java.util.List;
import java.util.Map;

public interface AlbumInterface {

    /* --- DTO Cast --- */

    AlbumInterface fromDto(final AlbumDto albumDto);
    AlbumDto toDto();

    /* --- PRIMARY KEY --- */

    Long getId();
    AlbumInterface setId(final Long id);

    /* --- Artist --- */

    ArtistInterface getArtist();
    AlbumInterface setArtist(final ArtistInterface artist);

    /* --- Artwork URL --- */

    String getArtworkUrl();
    AlbumInterface setArtworkUrl(final String artworkUrl);

    /* --- Catalog No --- */

    String getCatalogNo();
    AlbumInterface setCatalogNo(final String catalogNo);

    /* --- Credit Artists --- */

    Map<? extends ArtistInterface, ArtistRole> getCreditArtists();
    AlbumInterface setCreditArtists(final Map<? extends ArtistInterface, ArtistRole> creditArtists);

    /* --- Disc Total --- */

    short getDiscTotal();
    AlbumInterface setDiscTotal(final short discTotal);

    /* --- Label --- */

    LabelInterface getLabel();
    AlbumInterface setLabel(final LabelInterface label);

    /* --- Name --- */

    String getName();
    AlbumInterface setName(final String name);

    /* --- Primary Genres --- */

    List<? extends GenreInterface> getPrimaryGenres();
    AlbumInterface setPrimaryGenres(final List<? extends GenreInterface> primaryGenres);

    /* --- Release Date --- */

    LocalDate getReleaseDate();
    AlbumInterface setReleaseDate(final LocalDate releaseDate);

    /* --- Secondary Genres --- */

    List<? extends GenreInterface> getSecondaryGenres();
    AlbumInterface setSecondaryGenres(final List<? extends GenreInterface> secondaryGenres);

    /* --- Songs --- */

    List<? extends SongInterface> getSongs();
    AlbumInterface setSongs(final List<? extends SongInterface> songs);

    /* --- Track Total --- */

    short getTrackTotal();
    AlbumInterface setTrackTotal(final short trackTotal);

    /* --- Type --- */

    AlbumType getType();
    AlbumInterface setType(final AlbumType type);

}
