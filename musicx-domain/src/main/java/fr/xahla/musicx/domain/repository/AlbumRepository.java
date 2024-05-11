package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriterias;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.AlbumEntity;
import fr.xahla.musicx.domain.model.entity.ArtistEntity;
import fr.xahla.musicx.domain.model.entity.SongEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.database.HibernateLoader.openSession;
import static fr.xahla.musicx.domain.repository.ArtistRepository.artistRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.domain.repository.LabelRepository.labelRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public class AlbumRepository implements AlbumRepositoryInterface {

    private static final AlbumRepository INSTANCE = new AlbumRepository();

    /* ------------ Relations --------------- */

    @Override public ArtistDto getArtist(final AlbumDto album) {
        return artistRepository().find(album.getArtistId());
    }

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

    @Override public LabelDto getLabel(final AlbumDto album) {
        return labelRepository().find(album.getLabelId());
    }

    @Override public List<GenreDto> getPrimaryGenres(final AlbumDto album) {
        final var genres = new ArrayList<GenreDto>();

        album.getPrimaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    @Override public List<GenreDto> getSecondaryGenres(final AlbumDto album) {
        final var genres = new ArrayList<GenreDto>();

        album.getSecondaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    @Override public List<SongDto> getSongs(final AlbumDto album) {
        return songRepository().findByCriteria(Map.of(
            SongSearchCriterias.ALBUM, album.getId()
        ));
    }

    /* ------------ Selectors --------------- */

    @Override public AlbumDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(AlbumEntity.class, id).toDto();
        } catch (final HibernateException e) {
            logger().warning("Album not found with id: " + id);
            return null;
        }
    }

    @Override public List<AlbumDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(AlbumEntity.class)
        );
    }

    public List<AlbumEntity> findAllStructured() {
        return QueryHelper.findAll(AlbumEntity.class).stream()
            .map(AlbumEntity.class::cast)
            .toList();
    }

    @Override public List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                AlbumEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

    public List<AlbumEntity> findByCriteriaStructured(final Map<AlbumSearchCriterias, Object> criteria) {
        return QueryHelper.findByCriteria(
                ArtistEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            ).stream()
                .map(AlbumEntity.class::cast)
                .toList();
    }

    private List<AlbumDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(AlbumEntity.class::isInstance)
            .map(AlbumEntity.class::cast)
            .map(AlbumEntity::toDto)
            .toList();
    }

    @Override public void save(final AlbumDto album) {
        AlbumEntity albumEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != album.getId()
                    && null != (albumEntity = session.get(AlbumEntity.class, album.getId()))) {
                albumEntity.fromDto(album);
                session.merge(albumEntity);
            } else {
                albumEntity = new AlbumEntity().fromDto(album);
                session.persist(albumEntity);
                album.setId(albumEntity.getId());
            }

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + album.getName(), exception);
        }
    }

    public static AlbumRepository albumRepository() {
        return INSTANCE;
    }
}
