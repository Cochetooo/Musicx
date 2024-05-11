package fr.xahla.musicx.domain.model.entity;

import fr.xahla.musicx.api.model.LabelDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.Hibernate;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name="label")
@Getter
@Setter
public class LabelEntity {

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

    public LabelEntity fromDto(final LabelDto labelDto) {
        this.setId(labelDto.getId());
        this.setName(labelDto.getName());

        if (null != labelDto.getGenreIds()) {
            genres = new ArrayList<>();
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

    public LabelDto toDto() {
        final var labelDto = LabelDto.builder()
            .id(id)
            .name(name)
            .build();

        if (null != genres && Hibernate.isInitialized(genres)) {
            labelDto.setGenreIds(genres.stream()
                .map(GenreEntity::getId)
                .toList()
            );
        }

        return labelDto;
    }
}
