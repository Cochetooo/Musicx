package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.infrastructure.model.data.LibraryInterface;
import fr.xahla.musicx.infrastructure.repository.data.LibraryRepositoryInterface;
import fr.xahla.musicx.infrastructure.model.entity.Library;
import org.hibernate.Transaction;

import static fr.xahla.musicx.infrastructure.config.HibernateLoader.openSession;
import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Class that defines the repository for the Library model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class LibraryRepository implements LibraryRepositoryInterface {

    private final SongRepository songRepository;

    public LibraryRepository() {
        super();
        this.songRepository = new SongRepository();
    }

    @Override public void clear() {
        try (var session = openSession()) {
            var hql = "TRUNCATE TABLE libraries CASCADE";
            session.createNativeQuery(hql, Library.class).executeUpdate();
        }
    }

    @Override public Library findOneById(final Long id) {
        Library library;

        try (var session = openSession()) {
            var hql =
                "FROM Library le " +
                    "LEFT JOIN FETCH le.songs AS songs " +
                    "JOIN FETCH songs.artist AS artist " +
                    "WHERE le.id = :id";
            var query = session.createQuery(hql, Library.class);
            query.setParameter("id", id);
            library = query.uniqueResult();
        }

        return library;
    }

    @Override public Library findOneByName(final String name) {
        Library library;

        try (var session = openSession()) {
            var hql =
                "FROM Library le " +
                "LEFT JOIN FETCH le.songs AS songs " +
                "JOIN FETCH songs.album as album " +
                "JOIN FETCH album.artist as albumArtist " +
                "JOIN FETCH songs.artist AS artist " +
                "WHERE le.name = :name";
            var query = session.createQuery(hql, Library.class);
            query.setParameter("name", name);
            library = query.uniqueResult();
        }

        return library;
    }

    @Override public void save(final LibraryInterface libraryInterface) {
        for (var song : libraryInterface.getSongs()) {
            this.songRepository.save(song);
        }

        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            Library library;

            if (null != libraryInterface.getId() && null != (library = session.get(Library.class, libraryInterface.getId()))) {
                library.set(libraryInterface);
                session.merge(library);
            } else {
                library = new Library().set(libraryInterface);
                session.persist(library);
                libraryInterface.setId(library.getId());
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
