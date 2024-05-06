package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriterias;
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

public class ArtistRepository implements ArtistRepositoryInterface {

    private static final ArtistRepository INSTANCE = new ArtistRepository();

    @Override public List<AlbumDto> getAlbums(final ArtistDto artist) {
        final var sql = "FROM AlbumEntity a WHERE a.artist.id = :artist";

        try (final var session = openSession()) {
            final var query = session.createQuery(sql, AlbumEntity.class);
            query.setParameter("artist", artist.getId());

            return query.list().stream()
                .map(AlbumEntity::toDto)
                .toList();
        }
    }

    @Override public List<SongDto> getSongs(final ArtistDto artist) {
        final var sql = "FROM SongEntity s WHERE s.artist.id = :artist";

        try (final var session = openSession()) {
            final var query = session.createQuery(sql, SongEntity.class);
            query.setParameter("artist", artist.getId());

            return query.list().stream()
                .map(SongEntity::toDto)
                .toList();
        }
    }

    @Override public List<ArtistDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(ArtistEntity.class)
        );
    }

    @Override public List<ArtistDto> findByCriteria(final Map<ArtistSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                ArtistEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

    @Override public List<ArtistDto> findById(final Long id) {
        return this.findByCriteria(Map.of(ArtistSearchCriterias.ID, id));
    }

    private List<ArtistDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(ArtistEntity.class::isInstance)
            .map(ArtistEntity.class::cast)
            .map(ArtistEntity::toDto)
            .toList();
    }

    @Override public void save(final ArtistDto artist) {
        ArtistEntity artistEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != artist.getId()
                && null != (artistEntity = session.get(ArtistEntity.class, artist.getId()))) {
                artistEntity.fromDto(artist);
                session.merge(artistEntity);
            } else {
                artistEntity = new ArtistEntity().fromDto(artist);
                session.persist(artistEntity);
                artist.setId(artistEntity.getId());
            }

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + artist.getName(), exception);
        }
    }

    public static ArtistRepository artistRepository() {
        return INSTANCE;
    }
}
