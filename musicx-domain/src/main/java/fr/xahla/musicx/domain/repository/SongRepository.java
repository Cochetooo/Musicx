package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriteria;
import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.model.entity.SongEntity;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.QueryHelper.query;

/**
 * Manipulate song data with Hibernate.
 * @author Cochetooo
 * @since 0.1.0
 */
public class SongRepository implements SongRepositoryInterface {

    /* ------------ Relations --------------- */

    /**
     * @since 0.3.0
     */
    @Override public AlbumDto getAlbum(final SongDto song) {
        return albumRepository().find(song.getAlbumId());
    }

    /**
     * @since 0.3.0
     */
    @Override public ArtistDto getArtist(final SongDto song) {
        return artistRepository().find(song.getArtistId());
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getPrimaryGenres(final SongDto song) {
        final var genres = new ArrayList<GenreDto>();

        song.getPrimaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getSecondaryGenres(final SongDto song) {
        final var genres = new ArrayList<GenreDto>();

        song.getSecondaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /* ------------ Selectors --------------- */

    /**
     * @return The SongDto with the correspond id, otherwise <b>null</b>.
     * @since 0.3.0
     */
    @Override public SongDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(SongEntity.class, id).toDto();
        } catch (final Exception e) {
            log(LogMessage.WARNING_REPOSITORY_ITEM_NOT_FOUND, "Song", "id", id);
            return null;
        }
    }

    /**
     * @since 0.1.0
     */
    @Override public List<SongDto> findAll() {
        return this.toDtoList(
            QueryHelper.query_find_all(SongEntity.class)
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public List<SongDto> findByCriteria(final Map<SongSearchCriteria, Object> criteria) {
        final var query = new QueryBuilder()
            .from(SongEntity.class);

        criteria.forEach((key, value) -> query.where(key.getColumn(), value));

        return this.toDtoList(
            query(query.build()).response()
        );
    }

    /**
     * @since 0.1.0
     */
    @Override public void save(final SongDto song) {
        SongEntity songEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != song.getId()
                && null != (songEntity = session.get(SongEntity.class, song.getId()))) {
                songEntity.fromDto(song);
                session.merge(songEntity);
            } else {
                songEntity = new SongEntity().fromDto(song);
                session.persist(songEntity);
                song.setId(songEntity.getId());
            }

            transaction.commit();
            log(LogMessage.FINE_REPOSITORY_SAVE_SUCCESS, "Song", song.getTitle());
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            error(exception, LogMessage.ERROR_REPOSITORY_SAVE, song.getTitle());
        }
    }

    private List<SongDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(SongEntity.class::isInstance)
            .map(SongEntity.class::cast)
            .map(SongEntity::toDto)
            .toList();
    }
}
