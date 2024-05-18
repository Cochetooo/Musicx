package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.repository.ArtistRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriteria;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriteria;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriteria;
import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.ArtistEntity;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.QueryHelper.query;

/**
 * Manipulate artist data with Hibernate.
 * @author Cochetooo
 * @since 0.1.0
 */
public class ArtistRepository implements ArtistRepositoryInterface {

    /* ------------ Relations --------------- */

    /**
     * @since 0.3.0
     */
    @Override public List<PersonArtistDto> getMembers(final BandArtistDto artist) {
        final var members = new ArrayList<PersonArtistDto>();

        artist.getMemberIds().forEach(id -> members.add(
            (PersonArtistDto) this.find(id)
        ));

        return members;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<BandArtistDto> getBands(final PersonArtistDto artist) {
        final var bands = new ArrayList<BandArtistDto>();

        artist.getBandIds().forEach(id -> bands.add(
            (BandArtistDto) this.find(id)
        ));

        return bands;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<AlbumDto> getAlbums(final ArtistDto artist) {
        return albumRepository().findByCriteria(Map.of(
            AlbumSearchCriteria.ARTIST, artist.getId()
        ));
    }

    /**
     * @since 0.3.0
     */
    @Override public List<SongDto> getSongs(final ArtistDto artist) {
        return songRepository().findByCriteria(Map.of(
            SongSearchCriteria.ARTIST, artist.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    /**
     * @return The ArtistDto with the correspond id, otherwise <b>null</b>.
     * @since 0.3.0
     */
    @Override public ArtistDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(ArtistEntity.class, id).toDto();
        } catch (final Exception e) {
            logger().warn("REPOSITORY_ITEM_NOT_FOUND", "Artist", "id", id);
            return null;
        }
    }

    /**
     * @since 0.1.0
     */
    @Override public List<ArtistDto> findAll() {
        return this.toDtoList(
            QueryHelper.query_find_all(ArtistEntity.class)
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public List<ArtistDto> findByCriteria(final Map<ArtistSearchCriteria, Object> criteria) {
        final var query = new QueryBuilder()
            .from(ArtistEntity.class);

        criteria.forEach((key, value) -> query.where(key.getColumn(), value));

        return this.toDtoList(
            query(query.build()).response()
        );
    }

    /**
     * @since 0.1.0
     */
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
            logger().fine("REPOSITORY_SAVED", "Artist", artist.getName());
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().error(exception, "REPOSITORY_SAVE_ERROR", "Artist", artist.getName());
        }
    }

    /**
     * @since 0.3.0
     */
    private List<ArtistDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(ArtistEntity.class::isInstance)
            .map(ArtistEntity.class::cast)
            .map(ArtistEntity::toDto)
            .toList();
    }
}
