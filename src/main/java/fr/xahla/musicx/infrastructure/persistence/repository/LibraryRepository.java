package fr.xahla.musicx.infrastructure.persistence.repository;

import fr.xahla.musicx.api.data.LibraryInterface;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;
import fr.xahla.musicx.infrastructure.persistence.entity.ArtistEntity;
import fr.xahla.musicx.infrastructure.persistence.entity.SongEntity;
import fr.xahla.musicx.infrastructure.persistence.entity.LibraryEntity;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

import java.util.*;
import java.util.stream.Collectors;

public class LibraryRepository implements LibraryRepositoryInterface {

    private final SessionFactory sessionFactory;
    private final SongRepository songRepository;

    public LibraryRepository(SessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
        this.songRepository = new SongRepository(sessionFactory);
    }

    /**
     * Clear all entries from the table.
     */
    @Override public void clear() {
        try (var session = this.sessionFactory.openSession()) {
            var hql = "TRUNCATE TABLE libraries CASCADE";
            session.createNativeQuery(hql, LibraryEntity.class).executeUpdate();
        }
    }

    /**
     * Find a library in the database with the corresponding id.
     */
    @Override public LibraryEntity findOneById(Long id) {
        LibraryEntity library;

        try (var session = this.sessionFactory.openSession()) {
            var hql =
                "FROM LibraryEntity le " +
                    "LEFT JOIN FETCH le.songs AS songs " +
                    "JOIN FETCH songs.artist AS artist " +
                    "WHERE le.id = :id";
            var query = session.createQuery(hql, LibraryEntity.class);
            query.setParameter("id", id);
            library = query.uniqueResult();
        }

        return library;
    }

    /**
     * Find a library in the database with the corresponding name.
     */
    @Override public LibraryEntity findOneByName(String name) {
        LibraryEntity library;

        try (var session = this.sessionFactory.openSession()) {
            var hql =
                "FROM LibraryEntity le " +
                "LEFT JOIN FETCH le.songs AS songs " +
                "JOIN FETCH songs.album as album " +
                "JOIN FETCH album.artist as albumArtist " +
                "JOIN FETCH songs.artist AS artist " +
                "WHERE le.name = :name";
            var query = session.createQuery(hql, LibraryEntity.class);
            query.setParameter("name", name);
            library = query.uniqueResult();
        }

        return library;
    }

    /**
     * Save a library to the database.
     */
    @Override public void save(LibraryInterface library) {
        for (var song : library.getSongs()) {
            this.songRepository.save(song);
        }

        Transaction transaction = null;

        try (var session = this.sessionFactory.openSession()) {
            transaction = session.beginTransaction();

            LibraryEntity libraryEntity;

            if (null != library.getId() && null != (libraryEntity = session.get(LibraryEntity.class, library.getId()))) {
                libraryEntity.set(library);
                session.merge(libraryEntity);
            } else {
                libraryEntity = new LibraryEntity().set(library);
                session.persist(libraryEntity);
                library.setId(libraryEntity.getId());
            }

            transaction.commit();
        } catch (Exception e) {
            if (null != transaction) {
                transaction.rollback();
            }

            e.printStackTrace();
        }
    }
}
