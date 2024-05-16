package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriteria;
import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.model.entity.GenreEntity;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.QueryHelper.query;

/**
 * Manipulate genre data with Hibernate.
 * @author Cochetooo
 * @since 0.3.0
 */
public class GenreRepository implements GenreRepositoryInterface {

    /* ------------ Relations --------------- */

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getParents(final GenreDto genre) {
        final var parents = new ArrayList<GenreDto>();

        genre.getParentIds().forEach(id -> parents.add(this.find(id)));

        return parents;
    }

    /* ------------ Selectors --------------- */

    /**
     * @return The GenreDto with the correspond id, otherwise <b>null</b>.
     * @since 0.3.0
     */
    @Override public GenreDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(GenreEntity.class, id).toDto();
        } catch (final Exception e) {
            log(LogMessage.WARNING_REPOSITORY_ITEM_NOT_FOUND, "Genre", "id", id);
            return null;
        }
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> findAll() {
        return this.toDtoList(
            QueryHelper.query_find_all(GenreEntity.class)
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> findByCriteria(final Map<GenreSearchCriteria, Object> criteria) {
        final var query = new QueryBuilder()
            .from(GenreEntity.class);

        criteria.forEach((key, value) -> query.where(key.getColumn(), value));

        return this.toDtoList(
            query(query.build()).response()
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public void save(final GenreDto genre) {
        GenreEntity genreEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != genre.getId()
                && null != (genreEntity = session.get(GenreEntity.class, genre.getId()))) {
                genreEntity.fromDto(genre);
                session.merge(genreEntity);
            } else {
                genreEntity = new GenreEntity().fromDto(genre);
                session.persist(genreEntity);
                genre.setId(genreEntity.getId());
            }

            transaction.commit();
            log(LogMessage.FINE_REPOSITORY_SAVE_SUCCESS, "Genre", genre.getName());
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            error(exception, LogMessage.ERROR_REPOSITORY_SAVE, genre.getName());
        }
    }

    private List<GenreDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .map(GenreEntity::toDto)
            .toList();
    }
}
