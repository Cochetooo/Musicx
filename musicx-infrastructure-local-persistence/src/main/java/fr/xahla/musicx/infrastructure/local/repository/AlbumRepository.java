package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.infrastructure.local.model.AlbumEntity;
import fr.xahla.musicx.infrastructure.local.model.SongEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.List;
import java.util.Map;
import java.util.logging.Level;

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

    @Override public List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteriaMap) {
        final var sql = new StringBuilder("FROM AlbumEntity");

        final var conditions = criteriaMap.entrySet().stream()
            .map(entry -> entry.getKey().getColumn() + " = '" + entry.getValue() + "'")
            .toList();

        if (!conditions.isEmpty()) {
            sql.append(" WHERE ")
                .append(String.join(" AND ", conditions));
        }

        return this.query(sql.toString());
    }

    @Override public List<AlbumDto> findAll() {
        return this.query("FROM AlbumEntity");
    }

    @Override public List<AlbumDto> fromSongs(final List<SongDto> songs) {
        return songs.stream()
            .map(SongDto::getAlbum)
            .distinct()
            .toList();
    }

    private List<AlbumDto> query(final String query) {
        try (final var session = openSession()){
            final var result = session.createQuery(query, AlbumEntity.class);

            return result.list().stream()
                .map(AlbumEntity::toDto)
                .toList();
        }
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
        } catch (final HibernateException exception) {
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
