package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.infrastructure.local.helper.QueryHelper;
import fr.xahla.musicx.infrastructure.local.model.AlbumEntity;
import fr.xahla.musicx.infrastructure.local.model.ArtistEntity;
import fr.xahla.musicx.infrastructure.local.model.SongEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;

public class AlbumRepository implements AlbumRepositoryInterface {

    private static final AlbumRepository INSTANCE = new AlbumRepository();

    @Override public List<SongDto> getSongs(final AlbumDto album) {
        final var sql = "FROM SongEntity s WHERE s.album.id = :album";

        try (final var session = openSession()) {
            final var query = session.createQuery(sql, SongEntity.class);
            query.setParameter("album", album.getId());

            return query.list().stream()
                .map(SongEntity::toDto)
                .toList();
        }
    }

    @Override public List<AlbumDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(AlbumEntity.class)
        );
    }

    public List<AlbumEntity> findAllStructured() {
        return QueryHelper.findAll(AlbumEntity.class).stream()
            .map(AlbumEntity.class::cast)
            .toList();
    }

    @Override public List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                AlbumEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

    public List<AlbumEntity> findByCriteriaStructured(final Map<AlbumSearchCriterias, Object> criteria) {
        return QueryHelper.findByCriteria(
                ArtistEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            ).stream()
                .map(AlbumEntity.class::cast)
                .toList();
    }

    private List<AlbumDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(AlbumEntity.class::isInstance)
            .map(AlbumEntity.class::cast)
            .map(AlbumEntity::toDto)
            .toList();
    }

    @Override public void save(final AlbumDto album) {
        AlbumEntity albumEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != album.getId()
                    && null != (albumEntity = session.get(AlbumEntity.class, album.getId()))) {
                albumEntity.fromDto(album);
                session.merge(albumEntity);
            } else {
                albumEntity = new AlbumEntity().fromDto(album);
                session.persist(albumEntity);
                album.setId(albumEntity.getId());
            }

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + album.getName(), exception);
        }
    }

    public static AlbumRepository albumRepository() {
        return INSTANCE;
    }
}
