package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.data.GenreInterface;
import jakarta.persistence.*;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name="genre")
public class GenreEntity implements GenreInterface {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name")
    private String name;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(
        name="genre_parents",
        joinColumns = @JoinColumn(name = "genre_id"),
        inverseJoinColumns = @JoinColumn(name = "parent_genre_id")
    )
    private List<GenreEntity> parents;

    // Casts

    @Override public GenreEntity fromDto(final GenreDto genreDto) {
        this.setId(genreDto.getId())
            .setName(genreDto.getName());

        if (null != genreDto.getParentIds()) {
            parents = new ArrayList<>();
            Hibernate.initialize(parents);
            genreDto.getParentIds().forEach(genreId -> {
                final var genre = new GenreEntity();
                genre.setId(genreId);
                this.parents.add(genre);
            });
        }

        return this;
    }

    @Override public GenreDto toDto() {
        final var genreDto = new GenreDto()
            .setId(id)
            .setName(name);

        if (null != parents && Hibernate.isInitialized(parents)) {
            genreDto.setParentIds(parents.stream()
                .map(GenreEntity::getId)
                .toList()
            );
        }

        return genreDto;
    }

    // Getters - Setters

    public Long getId() {
        return id;
    }

    public GenreEntity setId(final Long id) {
        this.id = id;
        return this;
    }

    public String getName() {
        return name;
    }

    public GenreEntity setName(final String name) {
        this.name = name;
        return this;
    }

    public List<GenreEntity> getParents() {
        return parents;
    }

    public GenreEntity setParents(final List<? extends GenreInterface> parents) {
        this.parents = parents.stream()
            .filter(GenreEntity.class::isInstance)
            .map(GenreEntity.class::cast)
            .toList();

        return this;
    }
}
