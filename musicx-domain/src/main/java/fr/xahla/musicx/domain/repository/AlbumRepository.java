package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriteria;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriteria;
import fr.xahla.musicx.domain.database.QueryBuilder;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.AlbumEntity;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.QueryHelper.query;

/**
 * Manipulate album data with Hibernate.
 * @author Cochetooo
 * @since 0.1.0
 */
public class AlbumRepository implements AlbumRepositoryInterface {

    /* ------------ Relations --------------- */

    /**
     * @since 0.3.0
     */
    @Override public ArtistDto getArtist(final AlbumDto album) {
        return artistRepository().find(album.getArtistId());
    }

    /**
     * @since 0.3.0
     */
    @Override public Map<ArtistDto, ArtistRole> getCreditArtists(final AlbumDto album) {
        final var creditArtists = new HashMap<ArtistDto, ArtistRole>();

        try (final var session = openSession()) {
            final var results = session.createQuery(
                "SELECT aca.artist_id, aca.role FROM Album_Credit_Artists aca WHERE aca.album_id = :albumId",
                Object[].class
            ).setParameter("albumId", album.getId()).list();

            results.forEach((result) -> {
                creditArtists.put(
                    (ArtistDto) result[0],
                    (ArtistRole) result[1]
                );
            });
        }

        return creditArtists;
    }

    /**
     * @since 0.3.0
     */
    @Override public LabelDto getLabel(final AlbumDto album) {
        return labelRepository().find(album.getLabelId());
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getPrimaryGenres(final AlbumDto album) {
        final var genres = new ArrayList<GenreDto>();

        album.getPrimaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<GenreDto> getSecondaryGenres(final AlbumDto album) {
        final var genres = new ArrayList<GenreDto>();

        album.getSecondaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /**
     * @since 0.3.0
     */
    @Override public List<SongDto> getSongs(final AlbumDto album) {
        return songRepository().findByCriteria(Map.of(
            SongSearchCriteria.ALBUM, album.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    /**
     * @return The AlbumDto with the correspond id, otherwise <b>null</b>.
     * @since 0.3.0
     */
    @Override public AlbumDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(AlbumEntity.class, id).toDto();
        } catch (final Exception e) {
            logger().warn("REPOSITORY_ITEM_NOT_FOUND", "Album", "id", id);
            return null;
        }
    }

    /**
     * @since 0.1.0
     */
    @Override public List<AlbumDto> findAll() {
        return this.toDtoList(
            QueryHelper.query_find_all(AlbumEntity.class)
        );
    }

    /**
     * @since 0.3.0
     */
    @Override public List<AlbumDto> findByCriteria(final Map<AlbumSearchCriteria, Object> criteria) {
        final var query = new QueryBuilder()
            .from(AlbumEntity.class);

        criteria.forEach((key, value) -> query.where(key.getColumn(), value));

        return this.toDtoList(
            query(query.build()).response()
        );
    }

    /**
     * @since 0.1.0
     */
    @Override public void save(final AlbumDto album) {
        this.saveAll(List.of(album));
    }

    /**
     * @since 0.5.0
     */
    @Override public void saveAll(final List<AlbumDto> albums) {
        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            albums.forEach(album -> {
                AlbumEntity albumEntity;

                if (null != album.getId()
                        && null != (albumEntity = session.get(AlbumEntity.class, album.getId()))) {
                    albumEntity.fromDto(album);
                    session.merge(albumEntity);
                } else {
                    albumEntity = new AlbumEntity().fromDto(album);
                    session.persist(albumEntity);
                    album.setId(albumEntity.getId());
                }

                logger().fine("REPOSITORY_SAVED", "Album", album.getName());
            });

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().error(exception, "REPOSITORY_SAVE_ERROR", "Albums", "");
        }
    }

    /**
     * @since 0.3.0
     */
    private List<AlbumDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(AlbumEntity.class::isInstance)
            .map(AlbumEntity.class::cast)
            .map(AlbumEntity::toDto)
            .toList();
    }
}
