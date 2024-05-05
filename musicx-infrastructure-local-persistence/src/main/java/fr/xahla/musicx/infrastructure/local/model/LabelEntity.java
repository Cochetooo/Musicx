package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.model.data.AlbumInterface;
import fr.xahla.musicx.api.model.data.GenreInterface;
import fr.xahla.musicx.api.model.data.LabelInterface;
import jakarta.persistence.*;
import org.hibernate.Hibernate;

import java.util.List;

@Entity
@Table(name="label")
public class LabelEntity implements LabelInterface {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name")
    private String name;

    @ManyToMany
    @JoinTable(
        name="label_genres",
        joinColumns = @JoinColumn(name = "label_id"),
        inverseJoinColumns = @JoinColumn(name = "genre_id")
    )
    private List<GenreEntity> genres;

    @OneToMany(mappedBy = "label")
    private List<AlbumEntity> releases;

    @Override public LabelEntity fromDto(final LabelDto labelDto) {
        this.setId(labelDto.getId())
            .setName(labelDto.getName());

        if (!labelDto.getGenreIds().isEmpty()) {
            Hibernate.initialize(genres);
            genres.clear();
            labelDto.getGenreIds().forEach(genreId -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.genres.add(genre);
            });
        }

        return this;
    }

    @Override public LabelDto toDto() {
        final var labelDto = new LabelDto()
            .setId(this.getId())
            .setName(this.getName());

        if (!genres.isEmpty()) {
            labelDto.setGenreIds(genres.stream()
                .map(GenreEntity::getId)
                .toList()
            );
        }

        return labelDto;
    }

    // Getters - Setters

    public Long getId() {
        return id;
    }

    public LabelEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public LabelEntity setName(final String name) {
        this.name = name;
        return this;
    }

    @Override public List<AlbumEntity> getReleases() {
        return releases;
    }

    @Override public LabelEntity setReleases(final List<? extends AlbumInterface> releases) {
        this.releases = releases.stream()
            .filter(AlbumEntity.class::isInstance)
            .map(AlbumEntity.class::cast)
            .toList();

        return this;
    }

    public List<GenreEntity> getGenres() {
        return genres;
    }

    public LabelEntity setGenres(final List<? extends GenreInterface> genres) {
        this.genres = genres.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }
}
