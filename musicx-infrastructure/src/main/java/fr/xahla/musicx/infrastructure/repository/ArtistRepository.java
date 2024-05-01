package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
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

    @Override public fr.xahla.musicx.infrastructure.model.entity.ArtistDto findById(final Integer id) {
        fr.xahla.musicx.infrastructure.model.entity.ArtistDto artist;

        try (var session = openSession()) {
            final var hql =
                """
                FROM ArtistDto a
                WHERE a.id = :id
                """;

            final var query = session.createQuery(hql, fr.xahla.musicx.infrastructure.model.entity.ArtistDto.class);
            query.setParameter("id", id);
            artist = query.uniqueResult();
        }

        return artist;
    }

    @Override public void save(final ArtistDto artistDto) {
        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            fr.xahla.musicx.infrastructure.model.entity.ArtistDto artist;

            if (null != artistDto.getId() && null != (artist = session.get(fr.xahla.musicx.infrastructure.model.entity.ArtistDto.class, artistDto.getId()))) {
                artist.set(artistDto);
                session.merge(artist);
            } else {
                artist = new fr.xahla.musicx.infrastructure.model.entity.ArtistDto().set(artistDto);
                session.persist(artist);
                artistDto.setId(artist.getId());
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
