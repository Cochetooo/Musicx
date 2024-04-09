package fr.xahla.musicx.core.repository;

import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.core.model.entity.Artist;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

import java.util.logging.Logger;

import static fr.xahla.musicx.core.config.HibernateLoader.openSession;
import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class ArtistRepository implements ArtistRepositoryInterface {

    @Override public Artist findById(final Integer id) {
        Artist artist;

        try (var session = openSession()) {
            final var hql =
                """
                FROM Artist a
                WHERE a.id = :id
                """;

            final var query = session.createQuery(hql, Artist.class);
            query.setParameter("id", id);
            artist = query.uniqueResult();
        }

        return artist;
    }

    @Override public void save(final ArtistInterface artistInterface) {
        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            Artist artist;

            if (null != artistInterface.getId() && null != (artist = session.get(Artist.class, artistInterface.getId()))) {
                artist.set(artistInterface);
                session.merge(artist);
            } else {
                artist = new Artist().set(artistInterface);
                session.persist(artist);
                artistInterface.setId(artist.getId());
            }

            transaction.commit();
        } catch (Exception e) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().severe(e.getLocalizedMessage());
        }
    }
}
