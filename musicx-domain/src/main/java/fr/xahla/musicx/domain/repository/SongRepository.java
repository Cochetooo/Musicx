package fr.xahla.musicx.domain.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.SongSearchCriterias;
import fr.xahla.musicx.domain.helper.QueryHelper;
import fr.xahla.musicx.domain.model.entity.SongEntity;
import org.hibernate.Hibernate;
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
import static fr.xahla.musicx.domain.repository.ArtistRepository.artistRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.domain.repository.LabelRepository.labelRepository;

public class SongRepository implements SongRepositoryInterface {

    public static final SongRepository INSTANCE = new SongRepository();

    /* ------------ Relations --------------- */

    @Override public AlbumDto getAlbum(final SongDto song) {
        return albumRepository().find(song.getAlbumId());
    }

    @Override public ArtistDto getArtist(final SongDto song) {
        return artistRepository().find(song.getArtistId());
    }

    @Override public List<GenreDto> getPrimaryGenres(final SongDto song) {
        final var genres = new ArrayList<GenreDto>();

        song.getPrimaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    @Override public List<GenreDto> getSecondaryGenres(final SongDto song) {
        final var genres = new ArrayList<GenreDto>();

        song.getSecondaryGenreIds().forEach(id -> genres.add(
            genreRepository().find(id)
        ));

        return genres;
    }

    /* ------------ Selectors --------------- */

    @Override public SongDto find(final Long id) {
        try (final var session = openSession()) {
            return session.get(SongEntity.class, id).toDto();
        } catch (final Exception e) {
            logger().warning("Song not found with id: " + id);
            return null;
        }
    }

    @Override public List<SongDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(SongEntity.class)
        );
    }

    @Override public List<SongDto> findByCriteria(final Map<SongSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                SongEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

    @Override public void save(final SongDto song) {
        SongEntity songEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != song.getId()
                && null != (songEntity = session.get(SongEntity.class, song.getId()))) {
                songEntity.fromDto(song);
                session.merge(songEntity);
            } else {
                songEntity = new SongEntity().fromDto(song);
                session.persist(songEntity);
                song.setId(songEntity.getId());
            }

            transaction.commit();
            logger().fine("Song saved successfully: " + song.getTitle());
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + song.getTitle(), exception);
        }
    }

    private List<SongDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(SongEntity.class::isInstance)
            .map(SongEntity.class::cast)
            .map(SongEntity::toDto)
            .toList();
    }

    public static SongRepository songRepository() {
        return INSTANCE;
    }
}
