package fr.xahla.musicx.infrastructure.persistence.repository;

import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.domain.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.infrastructure.persistence.entity.ArtistEntity;
import fr.xahla.musicx.infrastructure.persistence.entity.SongEntity;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

public class ArtistRepository implements ArtistRepositoryInterface {

    private final SessionFactory sessionFactory;

    public ArtistRepository(
        final SessionFactory sessionFactory
    ) {
        this.sessionFactory = sessionFactory;
    }

    @Override public void save(ArtistInterface artist) {
        Transaction transaction = null;

        try (var session = this.sessionFactory.openSession()) {
            transaction = session.beginTransaction();

            ArtistEntity artistEntity;

            if (null != artist.getId() && null != (artistEntity = session.get(ArtistEntity.class, artist.getId()))) {
                artistEntity.set(artist);
                session.merge(artistEntity);
            } else {
                artistEntity = new ArtistEntity().set(artist);
                session.persist(artistEntity);
                artist.setId(artistEntity.getId());
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
