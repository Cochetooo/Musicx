package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.GenreDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;

/**
 * Genre persistence class for database
 * @author Cochetooo
 */
@Entity
@Table(name="genre")
@Getter
@Setter
public class GenreEntity {

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

    public GenreEntity fromDto(final GenreDto genreDto) {
        this.setId(genreDto.getId());
        this.setName(genreDto.getName());

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

    public GenreDto toDto() {
        final var genreDto = GenreDto.builder()
            .id(id)
            .name(name)
            .build();

        if (null != parents && Hibernate.isInitialized(parents)) {
            genreDto.setParentIds(parents.stream()
                .map(GenreEntity::getId)
                .toList()
            );
        }

        return genreDto;
    }
}
