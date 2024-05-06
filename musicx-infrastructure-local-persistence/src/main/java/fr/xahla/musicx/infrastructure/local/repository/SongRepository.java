package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriterias;
import fr.xahla.musicx.infrastructure.local.helper.QueryHelper;
import fr.xahla.musicx.infrastructure.local.model.SongEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;
import static fr.xahla.musicx.infrastructure.local.repository.AlbumRepository.albumRepository;

public class SongRepository implements SongRepositoryInterface {

    public static final SongRepository INSTANCE = new SongRepository();

    @Override public List<SongDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(SongEntity.class)
        );
    }

    @Override public List<SongDto> findByCriteria(final Map<SongSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                SongEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

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
        } catch (final HibernateException exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + song.getTitle(), exception);
        }
    }

    private List<SongDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(SongEntity.class::isInstance)
            .map(SongEntity.class::cast)
            .map(SongEntity::toDto)
            .toList();
    }

    public static SongRepository songRepository() {
        return INSTANCE;
    }
}
