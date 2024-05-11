package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.LabelRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriterias;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.LabelEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.database.HibernateLoader.openSession;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;

public class LabelRepository implements LabelRepositoryInterface {

    private static final LabelRepository instance = new LabelRepository();

    /* ------------ Relations --------------- */

    @Override public List<GenreDto> getGenres(final LabelDto label) {
        final var genres = new ArrayList<GenreDto>();

        label.getGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    @Override public List<AlbumDto> getReleases(final LabelDto label) {
        return albumRepository().findByCriteria(Map.of(
            AlbumSearchCriterias.LABEL, label.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    @Override public LabelDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(LabelEntity.class, id).toDto();
        } catch (final HibernateException e) {
            logger().warning("Label not found with id: " + id);
            return null;
        }
    }

    @Override public List<LabelDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(LabelEntity.class)
        );
    }

    @Override public List<LabelDto> findByCriteria(final Map<LabelSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                LabelEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

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
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + label.getName(), exception);
        }
    }

    private List<LabelDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(LabelEntity.class::isInstance)
            .map(LabelEntity.class::cast)
            .map(LabelEntity::toDto)
            .toList();
    }

    public static LabelRepository labelRepository() {
        return instance;
    }
}
