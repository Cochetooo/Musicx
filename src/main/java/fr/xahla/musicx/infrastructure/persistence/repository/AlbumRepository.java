package fr.xahla.musicx.infrastructure.persistence.repository;

import fr.xahla.musicx.api.data.AlbumInterface;
import fr.xahla.musicx.domain.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.infrastructure.persistence.entity.AlbumEntity;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

public class AlbumRepository implements AlbumRepositoryInterface {

    private final ArtistRepository artistRepository;
    private final SessionFactory sessionFactory;

    public AlbumRepository(
        final SessionFactory sessionFactory
    ) {
        this.sessionFactory = sessionFactory;
        this.artistRepository = new ArtistRepository(sessionFactory);
    }

    @Override public void save(AlbumInterface album) {
        this.artistRepository.save(album.getArtist());

        Transaction transaction = null;

        try (var session = this.sessionFactory.openSession()) {
            transaction = session.beginTransaction();

            AlbumEntity albumEntity;

            if (null != album.getId() && null != (albumEntity = session.get(AlbumEntity.class, album.getId()))) {
                albumEntity.set(album);
                session.merge(albumEntity);
            } else {
                albumEntity = new AlbumEntity().set(album);
                session.persist(albumEntity);
                album.setId(albumEntity.getId());
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
