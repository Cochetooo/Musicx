package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.LabelRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.LabelSearchCriterias;
import fr.xahla.musicx.infrastructure.local.helper.QueryHelper;
import fr.xahla.musicx.infrastructure.local.model.LabelEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;

public class LabelRepository implements LabelRepositoryInterface {

    private static final LabelRepository instance = new LabelRepository();

    @Override public List<AlbumDto> getReleases() {
        return List.of();
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
        } catch (final HibernateException exception) {
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
