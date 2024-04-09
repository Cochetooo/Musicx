package fr.xahla.musicx.core.repository;

import fr.xahla.musicx.api.model.AlbumInterface;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.core.model.entity.Album;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

import java.util.logging.Logger;

import static fr.xahla.musicx.core.config.HibernateLoader.openSession;
import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class AlbumRepository implements AlbumRepositoryInterface {

    private final ArtistRepository artistRepository;

    public AlbumRepository() {
        this.artistRepository = new ArtistRepository();
    }

    @Override public void save(final AlbumInterface albumInterface) {
        this.artistRepository.save(albumInterface.getArtist());

        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            Album album;

            if (null != albumInterface.getId() && null != (album = session.get(Album.class, albumInterface.getId()))) {
                album.set(albumInterface);
                session.merge(album);
            } else {
                album = new Album().set(albumInterface);
                session.persist(album);
                albumInterface.setId(album.getId());
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
