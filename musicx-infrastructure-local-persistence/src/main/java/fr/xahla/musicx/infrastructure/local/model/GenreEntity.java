package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.GenreDto;
import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name="genre")
public class GenreEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name")
    private String name;

    @ManyToMany
    @JoinTable(
        name="genre_parents",
        joinColumns = @JoinColumn(name = "genre_id"),
        inverseJoinColumns = @JoinColumn(name = "parent_genre_id")
    )
    private List<GenreEntity> parents;

    public GenreDto toDto() {
        return new GenreDto()
            .setName(name)
            .setParents(parents.stream().map(GenreEntity::toDto).toList());
    }

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

    public GenreEntity setParents(final List<GenreEntity> parents) {
        this.parents = parents;
        return this;
    }
}
