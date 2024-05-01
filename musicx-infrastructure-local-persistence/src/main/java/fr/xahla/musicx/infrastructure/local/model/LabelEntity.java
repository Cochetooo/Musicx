package fr.xahla.musicx.infrastructure.local.model;

import fr.xahla.musicx.api.model.LabelDto;
import jakarta.persistence.*;

import java.util.List;

@Entity
@Table(name="label")
public class LabelEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name")
    private String name;

    @OneToMany(mappedBy = "label")
    private List<GenreEntity> genres;

    public LabelDto toDto() {
        return new LabelDto()
            .setName(name)
            .setGenres(genres.stream().map(GenreEntity::toDto).toList());
    }

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

    public List<GenreEntity> getGenres() {
        return genres;
    }

    public LabelEntity setGenres(final List<GenreEntity> genres) {
        this.genres = genres;
        return this;
    }
}
