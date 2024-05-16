package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriteria;

import java.util.List;
import java.util.Map;

/**
 * Contracts to manipulate artists in database.
 * @author Cochetooo
 * @see ArtistDto
 * @since 0.2.0
 */
public interface ArtistRepositoryInterface {

    List<PersonArtistDto> getMembers(final BandArtistDto artist);
    List<BandArtistDto> getBands(final PersonArtistDto artist);

    List<AlbumDto> getAlbums(final ArtistDto artist);
    List<SongDto> getSongs(final ArtistDto artist);

    ArtistDto find(final Long id);
    List<ArtistDto> findAll();
    List<ArtistDto> findByCriteria(final Map<ArtistSearchCriteria, Object> criteria);

    void save(final ArtistDto artist);

}
