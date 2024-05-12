package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriterias;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.AlbumEntity;
import fr.xahla.musicx.domain.model.entity.ArtistEntity;
import fr.xahla.musicx.domain.model.entity.SongEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.database.HibernateLoader.openSession;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public class ArtistRepository implements ArtistRepositoryInterface {

    private static final ArtistRepository INSTANCE = new ArtistRepository();

    /* ------------ Relations --------------- */

    @Override public List<PersonArtistDto> getMembers(final BandArtistDto artist) {
        final var members = new ArrayList<PersonArtistDto>();

        artist.getMemberIds().forEach(id -> members.add(
            (PersonArtistDto) this.find(id)
        ));

        return members;
    }

    @Override public List<BandArtistDto> getBands(final PersonArtistDto artist) {
        final var bands = new ArrayList<BandArtistDto>();

        artist.getBandIds().forEach(id -> bands.add(
            (BandArtistDto) this.find(id)
        ));

        return bands;
    }

    @Override public List<AlbumDto> getAlbums(final ArtistDto artist) {
        return albumRepository().findByCriteria(Map.of(
            AlbumSearchCriterias.ARTIST, artist.getId()
        ));
    }

    @Override public List<SongDto> getSongs(final ArtistDto artist) {
        return songRepository().findByCriteria(Map.of(
            SongSearchCriterias.ARTIST, artist.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    @Override public ArtistDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(ArtistEntity.class, id).toDto();
        } catch (final Exception e) {
            logger().warning("Artist not found with id: " + id);
            return null;
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
            logger().fine("Artist saved successfully: " + artist.getName());
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
