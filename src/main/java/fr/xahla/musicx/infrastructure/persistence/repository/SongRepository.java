package fr.xahla.musicx.infrastructure.persistence.repository;

import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.domain.repository.SongRepositoryInterface;
import fr.xahla.musicx.infrastructure.persistence.entity.LibraryEntity;
import fr.xahla.musicx.infrastructure.persistence.entity.SongEntity;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;

public class SongRepository implements SongRepositoryInterface {

    private final ArtistRepository artistRepository;
    private final AlbumRepository albumRepository;
    private final SessionFactory sessionFactory;

    public SongRepository(
        final SessionFactory sessionFactory
    ) {
        this.sessionFactory = sessionFactory;
        this.albumRepository = new AlbumRepository(sessionFactory);
        this.artistRepository = new ArtistRepository(sessionFactory);
    }

    @Override public void save(SongInterface song) {
        this.artistRepository.save(song.getArtist());
        this.albumRepository.save(song.getAlbum());

        Transaction transaction = null;

        try (var session = this.sessionFactory.openSession()) {
            transaction = session.beginTransaction();

            SongEntity songEntity;

            if (null != song.getId() && null != (songEntity = session.get(SongEntity.class, song.getId()))) {
                songEntity.set(song);
                session.merge(songEntity);
            } else {
                songEntity = new SongEntity().set(song);
                session.persist(songEntity);
                song.setId(songEntity.getId());
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
