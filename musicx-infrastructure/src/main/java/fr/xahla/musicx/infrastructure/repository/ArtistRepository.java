package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.api.model.ArtistInterface;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.infrastructure.model.entity.Artist;
import org.hibernate.Transaction;

import static fr.xahla.musicx.infrastructure.config.HibernateLoader.openSession;
import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Class that defines the repository for the Artist model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
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
