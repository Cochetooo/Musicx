package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriteria;

import java.util.List;
import java.util.Map;

/**
 * Contracts to manipulate songs in database.
 * @author Cochetooo
 * @see SongDto
 * @since 0.2.0
 */
public interface SongRepositoryInterface {

    AlbumDto getAlbum(final SongDto song);
    ArtistDto getArtist(final SongDto song);
    List<GenreDto> getPrimaryGenres(final SongDto song);
    List<GenreDto> getSecondaryGenres(final SongDto song);

    SongDto find(final Long id);
    List<SongDto> findAll();
    List<SongDto> findByCriteria(final Map<SongSearchCriteria, Object> criteria);

    void save(final SongDto song);

}
