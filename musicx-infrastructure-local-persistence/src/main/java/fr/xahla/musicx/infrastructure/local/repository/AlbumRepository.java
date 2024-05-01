package fr.xahla.musicx.infrastructure.local.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.infrastructure.local.model.AlbumEntity;

import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.infrastructure.local.database.HibernateLoader.openSession;

public class AlbumRepository implements AlbumRepositoryInterface {

    private static final AlbumRepository INSTANCE = new AlbumRepository();

    @Override public List<SongDto> getSongs() {
        return List.of();
    }

    @Override public List<AlbumDto> findByCriteria(final Map<AlbumSearchCriterias, Object> criteriaMap) {
        final var sql = new StringBuilder("FROM AlbumEntity");

        final var conditions = criteriaMap.entrySet().stream()
            .map(entry -> entry.getKey().getColumn() + " = '" + entry.getValue() + "'")
            .toList();

        if (!conditions.isEmpty()) {
            sql.append(" WHERE ")
                .append(String.join(" AND ", conditions));
        }

        return this.query(sql.toString());
    }

    @Override public List<AlbumDto> findAll() {
        return this.query("FROM AlbumEntity");
    }

    @Override public List<AlbumDto> fromSongs(final List<SongDto> songs) {
        return this.query("");
    }

    @Override public List<AlbumDto> fromArtist(final ArtistDto artist) {
        return List.of();
    }

    @Override public List<AlbumDto> fromGenre(final GenreDto genre, final int mode) {
        return List.of();
    }

    private List<AlbumDto> query(final String query) {
        try (final var session = openSession()){
            final var result = session.createQuery(query, AlbumEntity.class);

            return result.list().stream()
                .map(AlbumEntity::toDto)
                .toList();
        }
    }

    @Override public void create(final AlbumDto album) {

    }

    public static AlbumRepository albumRepository() {
        return INSTANCE;
    }
}
