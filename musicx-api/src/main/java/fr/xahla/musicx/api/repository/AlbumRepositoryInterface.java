package fr.xahla.musicx.api.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriteria;

import java.util.List;
import java.util.Map;

/**
 * Contracts to manipulate albums in database.
 * @author Cochetooo
 * @see AlbumDto
 */
public interface AlbumRepositoryInterface {

    ArtistDto getArtist(final AlbumDto album);
    Map<ArtistDto, ArtistRole> getCreditArtists(final AlbumDto album);
    LabelDto getLabel(final AlbumDto album);
    List<GenreDto> getPrimaryGenres(final AlbumDto album);
    List<GenreDto> getSecondaryGenres(final AlbumDto album);

    List<SongDto> getSongs(final AlbumDto album);

    AlbumDto find(final Long id);
    List<AlbumDto> findByCriteria(final Map<AlbumSearchCriteria, Object> criteria);
    List<AlbumDto> findAll();

    void save(final AlbumDto album);

}
