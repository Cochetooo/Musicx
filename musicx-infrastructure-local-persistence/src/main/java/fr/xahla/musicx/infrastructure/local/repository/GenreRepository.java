package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.GenreRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;
import fr.xahla.musicx.infrastructure.local.helper.QueryHelper;
import fr.xahla.musicx.infrastructure.local.model.AlbumEntity;
import fr.xahla.musicx.infrastructure.local.model.ArtistEntity;
import fr.xahla.musicx.infrastructure.local.model.GenreEntity;
import org.hibernate.HibernateException;
import org.hibernate.Transaction;

import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.stream.Collectors;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;

public class GenreRepository implements GenreRepositoryInterface {

    public static final GenreRepository INSTANCE = new GenreRepository();

    @Override public List<GenreDto> findAll() {
        return this.toDtoList(
            QueryHelper.findAll(GenreEntity.class)
        );
    }

    public List<GenreEntity> findAllStructured() {
        return QueryHelper.findAll(GenreEntity.class).stream()
            .map(GenreEntity.class::cast)
            .toList();
    }

    @Override public List<GenreDto> findByCriteria(final Map<GenreSearchCriterias, Object> criteria) {
        return this.toDtoList(
            QueryHelper.findByCriteria(
                GenreEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            )
        );
    }

    public List<GenreEntity> findByCriteriaStructured(final Map<GenreSearchCriterias, Object> criteria) {
        return QueryHelper.findByCriteria(
                GenreEntity.class,
                criteria.entrySet().stream()
                    .collect(Collectors.toMap(
                        entry -> entry.getKey().getColumn(),
                        Map.Entry::getValue
                    ))
            ).stream()
            .map(GenreEntity.class::cast)
            .toList();
    }

    @Override public void save(final GenreDto genre) {
        GenreEntity genreEntity;

        Transaction transaction = null;

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            if (null != genre.getId()
                && null != (genreEntity = session.get(GenreEntity.class, genre.getId()))) {
                genreEntity.fromDto(genre);
                session.merge(genreEntity);
            } else {
                genreEntity = new GenreEntity().fromDto(genre);
                session.persist(genreEntity);
                genre.setId(genreEntity.getId());
            }

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().log(Level.SEVERE, "Error while persisting " + genre.getName(), exception);
        }
    }

    private List<GenreDto> toDtoList(final List<?> resultQuery) {
        return resultQuery.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .map(GenreEntity::toDto)
            .toList();
    }

    public static GenreRepository genreRepository() {
        return INSTANCE;
    }
}
