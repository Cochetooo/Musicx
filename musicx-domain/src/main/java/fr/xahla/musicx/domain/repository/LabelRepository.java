package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.LabelRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriteria;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriteria;
import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.LabelEntity;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.QueryHelper.query;

/**
 * Manipulate label data with Hibernate.
 * @author Cochetooo
 * @since 0.3.0
 */
public class LabelRepository implements LabelRepositoryInterface {

    /* ------------ Relations --------------- */

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getGenres(final LabelDto label) {
        final var genres = new ArrayList<GenreDto>();

        label.getGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<AlbumDto> getReleases(final LabelDto label) {
        return albumRepository().findByCriteria(Map.of(
            AlbumSearchCriteria.LABEL, label.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    /**
     * @return The LabelDto with the correspond id, otherwise <b>null</b>.
     * @since 0.3.0
     */
    @Override public LabelDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(LabelEntity.class, id).toDto();
        } catch (final Exception e) {
            logger().warn("REPOSITORY_ITEM_NOT_FOUND", "Label", "id", id);
            return null;
        }
    }

    /**
     * @since 0.3.0
     */
    @Override public List<LabelDto> findAll() {
        return this.toDtoList(
            QueryHelper.query_find_all(LabelEntity.class)
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public List<LabelDto> findByCriteria(final Map<LabelSearchCriteria, Object> criteria) {
        final var query = new QueryBuilder()
            .from(LabelEntity.class);

        criteria.forEach((key, value) -> query.where(key.getColumn(), value));

        return this.toDtoList(
            query(query.build()).response()
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public void save(final LabelDto label) {
        LabelEntity labelEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != label.getName()
                && null != (labelEntity = session.get(LabelEntity.class, label.getId()))) {
                labelEntity.fromDto(label);
                session.merge(labelEntity);
            } else {
                labelEntity = new LabelEntity().fromDto(label);
                session.persist(labelEntity);
            }

            transaction.commit();

            logger().fine("REPOSITORY_SAVED", "Label", label.getName());
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().error(exception, "REPOSITORY_SAVE_ERROR", "Label", label.getName());
        }
    }

    private List<LabelDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(LabelEntity.class::isInstance)
            .map(LabelEntity.class::cast)
            .map(LabelEntity::toDto)
            .toList();
    }
}
